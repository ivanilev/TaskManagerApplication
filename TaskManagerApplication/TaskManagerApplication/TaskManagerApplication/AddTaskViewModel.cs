using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Data.Entity.Validation;

namespace TaskManagerApplication
{
    public class AddTaskViewModel : INotifyPropertyChanged
    {
        private TaskManagerDBContext _dbContext = new TaskManagerDBContext();

        public AddTaskViewModel()
        {
            SaveChangesCommand = new RelayCommand(SaveTask);

            FillCategories();
        }

        private void FillCategories()
        {
            var q = (from c in _dbContext.Categories select c).ToList();
            Categories = q;
        }
        private void SaveTask(object o)
        {
            string errors = Validate();
            if (!String.IsNullOrEmpty(errors)) { MessageBox.Show(errors); return; }


            try
            {
                Task t = new Task();

                t.Name = TaskName;
                t.Category = SelectedCategory;
                t.Deadline = Deadline;
                t.Description = TaskDescription;

                if (isHighPriorityChecked) { t.PriorityID = 1; }
                else if (IsMediumPriorityChecked) { t.PriorityID = 2; }
                else { t.PriorityID = 3; }

                _dbContext.Tasks.Add(t);
                _dbContext.SaveChanges();
            }
            catch(DbEntityValidationException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private string Validate()
        {
            if (string.IsNullOrEmpty(TaskName) || string.IsNullOrWhiteSpace(TaskName))
            {
                return "Please select a task name!";
            }

            if ((IsHighPriorityChecked || IsMediumPriorityChecked || IsLowPriorityChecked) == false)
                return "Please select task priority!";
            
            if (SelectedCategory == null)
                return "Please select task category!";
            

            if (Deadline==null || Deadline <= DateTime.Today)
                return "Deadline can't be null or scheduled for a previous date!";

            if (String.IsNullOrEmpty(TaskDescription) || String.IsNullOrWhiteSpace(TaskDescription))
            {
                return "Please fill the task description field!";
            }

            return string.Empty;
        }


        #region Properties
        
        private bool isHighPriorityChecked;
        public bool IsHighPriorityChecked
        {
            get { return isHighPriorityChecked; }
            set { isHighPriorityChecked = value; NotifyPropertyChanged("IsHighPriorityChecked"); }
        }

        private bool isMediumPriorityChecked;
        public bool IsMediumPriorityChecked
        {
            get { return isMediumPriorityChecked; }
            set { isMediumPriorityChecked = value; NotifyPropertyChanged("IsMediumPriorityChecked"); }
        }

        private bool isLowPriorityChecked;
        public bool IsLowPriorityChecked
        {
            get { return isLowPriorityChecked; }
            set { isLowPriorityChecked = value; NotifyPropertyChanged("IsLowPriorityChecked"); }
        }

        private List<Category> categories;
        public List<Category> Categories
        {
            get { return categories; }
            set { categories = value; NotifyPropertyChanged("Categories"); }
        }


        private Category selectedCategory;
        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set { selectedCategory = value; NotifyPropertyChanged("SelectedCategory"); }
        }


        private string taskName;
        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; NotifyPropertyChanged("TaskName"); }
        }

        private string taskDescription;
        public string TaskDescription
        {
            get { return taskDescription; }
            set { taskDescription = value; NotifyPropertyChanged("TaskDescription"); }
        }

        private Nullable<DateTime> deadline = null;
        public Nullable<DateTime> Deadline
        {
            get { return deadline; }
            set { deadline = value; NotifyPropertyChanged("Deadline"); }
        }
        
        #endregion

        public ICommand SaveChangesCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
