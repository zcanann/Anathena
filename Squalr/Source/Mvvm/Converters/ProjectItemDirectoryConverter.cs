namespace Squalr.Source.Mvvm.Converters
{
    using Squalr.Engine.Common.DataStructures;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Projects.Items;
    using Squalr.Source.ProjectExplorer.ProjectItems;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    public class ProjectItemDirectoryConverter : IValueConverter
    {
        /// <summary>
        /// Structure to hold mappings of project items to views to prevent reallocation of view items. Prevents IsExpanded property from being reset.
        /// </summary>
        private static readonly Dictionary<ProjectItem, ProjectItemView> ViewMap = new Dictionary<ProjectItem, ProjectItemView>();

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value as IEnumerable<ProjectItem> != null)
            {
                FullyObservableCollection<ProjectItemView> projectItems = new FullyObservableCollection<ProjectItemView>();

                foreach (ProjectItem projectItem in value as IEnumerable<ProjectItem>)
                {
                    if (!ProjectItemDirectoryConverter.ViewMap.ContainsKey(projectItem))
                    {
                        ProjectItemView projectItemView = this.ConvertToProjectItemView(projectItem);
                        ProjectItemDirectoryConverter.ViewMap[projectItem] = projectItemView;
                    }

                    projectItems.Add(ProjectItemDirectoryConverter.ViewMap[projectItem]);
                }

                return projectItems;
            }

            return null;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private ProjectItemView ConvertToProjectItemView(ProjectItem projectItem)
        {
            switch (projectItem)
            {
                case DirectoryItem _ when projectItem is DirectoryItem:
                    return new DirectoryItemView(projectItem as DirectoryItem);
                case ProjectItem _ when projectItem is PointerItem:
                    return new PointerItemView(projectItem as PointerItem);
                case ProjectItem _ when projectItem is ScriptItem:
                    return new ScriptItemView(projectItem as ScriptItem);
                case ProjectItem _ when projectItem is InstructionItem:
                    return new InstructionItemView(projectItem as InstructionItem);
                case ProjectItem _ when projectItem is DotNetItem:
                    return new DotNetItemView(projectItem as DotNetItem);
                case ProjectItem _ when projectItem is JavaItem:
                    return new JavaItemView(projectItem as JavaItem);
                default:
                    Logger.Log(LogLevel.Error, "Unknown project item type");
                    return null;
            }
        }
    }
    //// End class
}
//// End namespace