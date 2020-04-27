namespace Squalr.Engine.Projects.Items
{
    using SharpDX.DirectInput;
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Input.HotKeys;
    using Squalr.Engine.Projects.Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;

    /// <summary>
    /// A base class for all project items that can be added to the project explorer.
    /// </summary>
    [KnownType(typeof(ProjectItem))]
    [KnownType(typeof(ScriptItem))]
    [KnownType(typeof(AddressItem))]
    [KnownType(typeof(InstructionItem))]
    [KnownType(typeof(PointerItem))]
    [KnownType(typeof(DotNetItem))]
    [KnownType(typeof(JavaItem))]
    [DataContract]
    public class ProjectItem : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// The name of this project item.
        /// </summary>
        [Browsable(false)]
        [DataMember]
        protected String name;

        /// <summary>
        /// The description of this project item.
        /// </summary>
        [Browsable(false)]
        [DataMember]
        protected String description;

        /// <summary>
        /// The unique identifier of this project item.
        /// </summary>
        [Browsable(false)]
        [DataMember]
        protected Guid guid;

        /// <summary>
        /// The hotkey associated with this project item.
        /// </summary>
        [Browsable(false)]
        protected Hotkey hotkey;

        /// <summary>
        /// A value indicating whether this project item has been activated.
        /// </summary>
        [Browsable(false)]
        protected Boolean isActivated;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem" /> class.
        /// </summary>
        internal ProjectItem() : this( String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem" /> class.
        /// </summary>
        /// <param name="name">The name of the project item.</param>
        internal ProjectItem(String name)
        {
            // Bypass setters/getters to avoid triggering any view updates in constructor
            this.name = name ?? String.Empty;
            this.isActivated = false;
            this.guid = Guid.NewGuid();
            this.ActivationLock = new Object();
            this.IsFileAssociated = false;
        }

        public static ProjectItem FromFile(String filePath, DirectoryItem parent)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new Exception("File does not exist: " + filePath);
                }

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    Type type = null;

                    switch ((new FileInfo(filePath).Extension).ToLower())
                    {
                        case ScriptItem.Extension:
                            type = typeof(ScriptItem);
                            break;
                        case PointerItem.Extension:
                            type = typeof(PointerItem);
                            break;
                        case InstructionItem.Extension:
                            type = typeof(InstructionItem);
                            break;
                        case DotNetItem.Extension:
                            type = typeof(DotNetItem);
                            break;
                        case JavaItem.Extension:
                            type = typeof(JavaItem);
                            break;
                        default:
                            return null;
                    }

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);

                    ProjectItem projectItem = serializer.ReadObject(fileStream) as ProjectItem;
                    projectItem.name = Path.GetFileNameWithoutExtension(filePath);
                    projectItem.Parent = parent;
                    projectItem.IsFileAssociated = true;

                    return projectItem;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error loading file", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Saves this project item by serializing it to disk.
        /// </summary>
        public virtual void Save()
        {
            try
            {
                using (FileStream fileStream = new FileStream(this.FullPath, FileMode.Create, FileAccess.Write))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(this.GetType());
                    serializer.WriteObject(fileStream, this);

                    this.IsFileAssociated = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error saving file", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Occurs after a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the parent of this project item.
        /// </summary>
        public DirectoryItem Parent { get; set; }

        /// <summary>
        /// Gets or sets the description for this object.
        /// </summary>
        public virtual String Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.Name == value || !this.Name.IsValidFileName())
                {
                    return;
                }

                this.name = this.ResolvePotentialNameConflict(this.Name, value);
                this.RaisePropertyChanged(nameof(this.Name));
                this.Save();
            }
        }

        /// <summary>
        /// Gets or sets the description for this object.
        /// </summary>
        public virtual String Description
        {
            get
            {
                return this.description;
            }

            set
            {
                if (this.description == value)
                {
                    return;
                }

                this.description = value;
                this.RaisePropertyChanged(nameof(this.Description));
                this.Save();
            }
        }

        /// <summary>
        /// Gets or sets the hotkey for this project item.
        /// </summary>
        public virtual Hotkey HotKey
        {
            get
            {
                return this.hotkey;
            }

            set
            {
                if (this.hotkey == value)
                {
                    return;
                }

                this.hotkey = value;
                this.HotKey?.SetCallBackFunction(() => this.IsActivated = !this.IsActivated);
                this.RaisePropertyChanged(nameof(this.HotKey));
                this.Save();
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of this project item.
        /// </summary>
        [Browsable(false)]
        public Guid Guid
        {
            get
            {
                return this.guid;
            }

            set
            {
                if (this.guid == value)
                {
                    return;
                }

                this.guid = value;
                this.RaisePropertyChanged(nameof(this.Guid));
                this.Save();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this item is activated.
        /// </summary>
        [Browsable(false)]
        public Boolean IsActivated
        {
            get
            {
                return this.isActivated;
            }

            set
            {
                lock (this.ActivationLock)
                {
                    if (this.isActivated == value)
                    {
                        return;
                    }

                    // Change activation state
                    Boolean previousValue = this.isActivated;
                    this.isActivated = value;
                    this.OnActivationChanged();

                    // Activation failed
                    if (this.isActivated == previousValue)
                    {
                        return;
                    }

                    this.RaisePropertyChanged(nameof(this.IsActivated));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if this project item is enabled.
        /// </summary>
        [Browsable(false)]
        public virtual Boolean IsEnabled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the display value to represent this project item.
        /// </summary>
        public virtual String DisplayValue
        {
            get
            {
                return String.Empty;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets the full path for this project item.
        /// </summary>
        [Browsable(false)]
        public String FullPath
        {
            get
            {
                return this.GetFilePathForName(this.Name);
            }
        }

        /// <summary>
        /// Gets the extension for this project item.
        /// </summary>
        public virtual String GetExtension()
        {
            return String.Empty;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a file on disk is associated with this project item.
        /// </summary>
        private Boolean IsFileAssociated { get; set; }

        /// <summary>
        /// Gets or sets a lock for activating project items.
        /// </summary>
        private Object ActivationLock { get; set; }

        /// <summary>
        /// Invoked when this object is deserialized.
        /// </summary>
        /// <param name="streamingContext">Streaming context.</param>
        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            if (this.Guid == null || this.Guid == Guid.Empty)
            {
                this.guid = Guid.NewGuid();
            }

            this.ActivationLock = new Object();
        }

        /// <summary>
        /// Updates this project item. Resolves addresses and values.
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// Clones the project item.
        /// </summary>
        /// <returns>The clone of the project item.</returns>
        public virtual ProjectItem Clone()
        {
            Byte[] serializedProjectItem;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ProjectItem));

            // Serialize this project item
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, this);
                serializedProjectItem = memoryStream.ToArray();
            }

            // Deserialize this project item to clone it
            using (MemoryStream memoryStream = new MemoryStream(serializedProjectItem))
            {
                return serializer.ReadObject(memoryStream) as ProjectItem;
            }
        }

        /// <summary>
        /// Updates the hotkey, bypassing setters to avoid triggering view updates.
        /// </summary>
        /// <param name="hotkey">The hotkey for this project item.</param>
        public void LoadHotkey(Hotkey hotkey)
        {
            this.hotkey = hotkey;

            this.HotKey?.SetCallBackFunction(() => this.IsActivated = !this.IsActivated);
        }

        public void Dispose()
        {
            this.HotKey?.Dispose();
        }

        /// <summary>
        /// Randomizes the guid of this project item.
        /// </summary>
        public void ResetGuid()
        {
            this.Guid = Guid.NewGuid();
        }

        /// <summary>
        /// Event received when a key is released.
        /// </summary>
        /// <param name="key">The key that was released.</param>
        public void OnKeyPress(Key key)
        {
        }

        /// <summary>
        /// Event received when a key is down.
        /// </summary>
        /// <param name="key">The key that is down.</param>
        public void OnKeyRelease(Key key)
        {
        }

        /// <summary>
        /// Event received when a key is down.
        /// </summary>
        /// <param name="key">The key that is down.</param>
        public void OnKeyDown(Key key)
        {
        }

        /// <summary>
        /// Event received when a set of keys are down.
        /// </summary>
        /// <param name="pressedKeys">The down keys.</param>
        public void OnUpdateAllDownKeys(HashSet<Key> pressedKeys)
        {
            if (this.HotKey is KeyboardHotkey)
            {
                KeyboardHotkey keyboardHotkey = this.HotKey as KeyboardHotkey;
            }
        }

        /// <summary>
        /// Indicates that a given property in this project item has changed.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected void RaisePropertyChanged(String propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Deactivates this item without triggering the <see cref="OnActivationChanged" /> function.
        /// </summary>
        protected void ResetActivation()
        {
            lock (this.ActivationLock)
            {
                this.isActivated = false;
                this.RaisePropertyChanged(nameof(this.IsActivated));
            }
        }

        /// <summary>
        /// Called when the activation state changes.
        /// </summary>
        protected virtual void OnActivationChanged()
        {
        }

        /// <summary>
        /// Overridable function indicating if this script can be activated.
        /// </summary>
        /// <returns>True if the script can be activated, otherwise false.</returns>
        protected virtual Boolean IsActivatable()
        {
            return true;
        }

        /// <summary>
        /// Resolves the name conflict for this unassociated project item.
        /// </summary>
        /// <returns></returns>
        private String ResolvePotentialNameConflict(String previousName, String newName)
        {
            if (this.IsFileAssociated && previousName == newName)
            {
                return newName;
            }

            String previousFilePath = this.GetFilePathForName(previousName);
            String newFilePath = this.GetFilePathForName(newName);

            if (File.Exists(newFilePath))
            {
                try
                {
                    String[] neighboringFiles = Directory.GetFiles(Path.GetDirectoryName(newFilePath));

                    for (Int32 appendedNumber = 0; appendedNumber < Int32.MaxValue; appendedNumber++)
                    {
                        String suffix = (appendedNumber == 0 ? String.Empty : " " + appendedNumber.ToString());
                        String resolvedName = Path.Combine(ProjectSettings.ProjectRoot, newName + suffix);

                        if (neighboringFiles.Contains(resolvedName))
                        {
                            continue;
                        }

                        // Rename name to resolved name
                        newName = resolvedName;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, "Error resolving conflicting project name.", ex);
                }
            }

            return newName;
        }

        private String GetFilePathForName(String fileName)
        {
            return Path.Combine((this.Parent?.FullPath ?? ProjectSettings.ProjectRoot), this.Name + this.GetExtension());
        }
    }
    //// End class
}
//// End namespace