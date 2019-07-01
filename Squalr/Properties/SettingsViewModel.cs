namespace Squalr.Properties
{
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Projects.Properties;
    using Squalr.Engine.Scanning.Properties;
    using Squalr.Source.Docking;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// View model for the Settings.
    /// </summary>
    internal class SettingsViewModel : ToolViewModel
    {
        /// <summary>
        /// Singleton instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        private static Lazy<SettingsViewModel> settingsViewModelInstance = new Lazy<SettingsViewModel>(
                () => { return new SettingsViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingsViewModel"/> class from being created.
        /// </summary>
        private SettingsViewModel() : base("Settings")
        {
            ProjectSettings.Default.PropertyChanged += ProjectSettingsPropertyChanged;
            ScanSettings.Default.PropertyChanged += ScanSettingsPropertyChanged;
            DockingViewModel.GetInstance().RegisterViewModel(this);
        }

        /// <summary>
        /// Gets or sets the root directory for all projects.
        /// </summary>
        public String ProjectRoot
        {
            get
            {
                String savedPath = ProjectSettings.Default.ProjectRoot;

                if (!Directory.Exists(savedPath))
                {
                    savedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Squalr");
                    this.ProjectRoot = savedPath;
                }

                return ProjectSettings.Default.ProjectRoot;
            }

            set
            {
                try
                {
                    if (!Directory.Exists(value))
                    {
                        Directory.CreateDirectory(value);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, "Unable to set project root", ex);
                }

                ProjectSettings.Default.ProjectRoot = value;
                this.RaisePropertyChanged(nameof(this.ProjectRoot));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not 'write' flags are required in retrieved virtual memory pages.
        /// </summary>
        public Boolean RequiredProtectionWrite
        {
            get
            {
                return ScanSettings.Default.RequiredWrite;
            }

            set
            {
                ScanSettings.Default.RequiredWrite = value;
                this.RaisePropertyChanged(nameof(this.RequiredProtectionWrite));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not 'execute' flags are required in retrieved virtual memory pages.
        /// </summary>
        public Boolean RequiredProtectionExecute
        {
            get
            {
                return ScanSettings.Default.RequiredExecute;
            }

            set
            {
                ScanSettings.Default.RequiredExecute = value;
                this.RaisePropertyChanged(nameof(this.RequiredProtectionExecute));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not 'copy on write' flags are required in retrieved virtual memory pages.
        /// </summary>
        public Boolean RequiredProtectionCopyOnWrite
        {
            get
            {
                return ScanSettings.Default.RequiredCopyOnWrite;
            }

            set
            {
                ScanSettings.Default.RequiredCopyOnWrite = value;
                this.RaisePropertyChanged(nameof(this.RequiredProtectionCopyOnWrite));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not retrieved virtual memory pages exclude those with 'write' flags.
        /// </summary>
        public Boolean ExcludedProtectionWrite
        {
            get
            {
                return ScanSettings.Default.ExcludedWrite;
            }

            set
            {
                ScanSettings.Default.ExcludedWrite = value;
                this.RaisePropertyChanged(nameof(this.ExcludedProtectionWrite));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not retrieved virtual memory pages exclude those with 'execute' flags.
        /// </summary>
        public Boolean ExcludedProtectionExecute
        {
            get
            {
                return ScanSettings.Default.ExcludedExecute;
            }

            set
            {
                ScanSettings.Default.ExcludedExecute = value;
                this.RaisePropertyChanged(nameof(this.ExcludedProtectionExecute));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not retrieved virtual memory pages exclude those with 'copy on write' flags.
        /// </summary>
        public Boolean ExcludedProtectionCopyOnWrite
        {
            get
            {
                return ScanSettings.Default.ExcludedCopyOnWrite;
            }

            set
            {
                ScanSettings.Default.ExcludedCopyOnWrite = value;
                this.RaisePropertyChanged(nameof(this.ExcludedProtectionCopyOnWrite));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not retrieved virtual memory pages allow 'none' memory type.
        /// </summary>
        public Boolean MemoryTypeNone
        {
            get
            {
                return ScanSettings.Default.MemoryTypeNone;
            }

            set
            {
                ScanSettings.Default.MemoryTypeNone = value;
                this.RaisePropertyChanged(nameof(this.MemoryTypeNone));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not retrieved virtual memory pages allow 'private' memory type.
        /// </summary>
        public Boolean MemoryTypePrivate
        {
            get
            {
                return ScanSettings.Default.MemoryTypePrivate;
            }

            set
            {
                ScanSettings.Default.MemoryTypePrivate = value;
                this.RaisePropertyChanged(nameof(this.MemoryTypePrivate));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not retrieved virtual memory pages allow 'mapped' memory type.
        /// </summary>
        public Boolean MemoryTypeMapped
        {
            get
            {
                return ScanSettings.Default.MemoryTypeMapped;
            }

            set
            {
                ScanSettings.Default.MemoryTypeMapped = value;
                this.RaisePropertyChanged(nameof(this.MemoryTypeMapped));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not retrieved virtual memory pages allow 'image' memory type.
        /// </summary>
        public Boolean MemoryTypeImage
        {
            get
            {
                return ScanSettings.Default.MemoryTypeImage;
            }

            set
            {
                ScanSettings.Default.MemoryTypeImage = value;
                this.RaisePropertyChanged(nameof(this.MemoryTypeImage));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not retrieved virtual memory pages must be in usermode range.
        /// </summary>
        public Boolean IsUserMode
        {
            get
            {
                return ScanSettings.Default.IsUserMode;
            }

            set
            {
                ScanSettings.Default.IsUserMode = value;
                this.RaisePropertyChanged(nameof(this.IsUserMode));
                this.RaisePropertyChanged(nameof(this.IsNotUserMode));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not retrieved virtual memory pages can be in any address range.
        /// </summary>
        public Boolean IsNotUserMode
        {
            get
            {
                return !ScanSettings.Default.IsUserMode;
            }

            set
            {
                ScanSettings.Default.IsUserMode = !value;
                this.RaisePropertyChanged(nameof(this.IsUserMode));
                this.RaisePropertyChanged(nameof(this.IsNotUserMode));
            }
        }

        /// <summary>
        /// Gets or sets a the interval of reupdating frozen values.
        /// </summary>
        public Int32 FreezeInterval
        {
            get
            {
                return ScanSettings.Default.FreezeInterval;
            }

            set
            {
                ScanSettings.Default.FreezeInterval = value;
                this.RaisePropertyChanged(nameof(this.FreezeInterval));
            }
        }

        /// <summary>
        /// Gets or sets a the interval between repeated scans.
        /// </summary>
        public Int32 RescanInterval
        {
            get
            {
                return ScanSettings.Default.RescanInterval;
            }

            set
            {
                ScanSettings.Default.RescanInterval = value;
                this.RaisePropertyChanged(nameof(this.RescanInterval));
            }
        }

        /// <summary>
        /// Gets or sets a the interval between reading scan results.
        /// </summary>
        public Int32 ResultReadInterval
        {
            get
            {
                return ScanSettings.Default.ResultReadInterval;
            }

            set
            {
                ScanSettings.Default.ResultReadInterval = value;
                this.RaisePropertyChanged(nameof(this.ResultReadInterval));
            }
        }

        /// <summary>
        /// Gets or sets a the interval between reading values in the table.
        /// </summary>
        public Int32 TableReadInterval
        {
            get
            {
                return ScanSettings.Default.TableReadInterval;
            }

            set
            {
                ScanSettings.Default.TableReadInterval = value;
                this.RaisePropertyChanged(nameof(this.TableReadInterval));
            }
        }

        /// <summary>
        /// Gets or sets a the allowed period of time for a given input to register as correlated with memory changes.
        /// </summary>
        public Int32 InputCorrelatorTimeOutInterval
        {
            get
            {
                return ScanSettings.Default.InputCorrelatorTimeOutInterval;
            }

            set
            {
                ScanSettings.Default.InputCorrelatorTimeOutInterval = value;
                this.RaisePropertyChanged(nameof(this.InputCorrelatorTimeOutInterval));
            }
        }

        /// <summary>
        /// Gets or sets the virtual memory alignment required in scans.
        /// </summary>
        public Int32 Alignment
        {
            get
            {
                return ScanSettings.Default.Alignment;
            }

            set
            {
                ScanSettings.Default.Alignment = value;
                this.RaisePropertyChanged(nameof(this.Alignment));
            }
        }

        /// <summary>
        /// Gets or sets the start address of virtual memory scans.
        /// </summary>
        public UInt64 StartAddress
        {
            get
            {
                return ScanSettings.Default.StartAddress;
            }

            set
            {
                ScanSettings.Default.StartAddress = value;
                this.RaisePropertyChanged(nameof(this.StartAddress));
            }
        }

        /// <summary>
        /// Gets or sets the end address of virtual memory scans.
        /// </summary>
        public UInt64 EndAddress
        {
            get
            {
                return ScanSettings.Default.EndAddress;
            }

            set
            {
                ScanSettings.Default.EndAddress = value;
                this.RaisePropertyChanged(nameof(this.EndAddress));
            }
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static SettingsViewModel GetInstance()
        {
            return SettingsViewModel.settingsViewModelInstance.Value;
        }

        private void ProjectSettingsPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            ProjectSettings.Default.Save();
        }

        private void ScanSettingsPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            ScanSettings.Default.Save();
        }
    }
    //// End class
}
//// End namespace