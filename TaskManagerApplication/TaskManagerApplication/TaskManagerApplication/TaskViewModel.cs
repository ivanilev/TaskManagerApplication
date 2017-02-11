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
    public class TaskViewModel : INotifyPropertyChanged
    {
        private TaskManagerDBContext _dbContext = new TaskManagerDBContext();

        public TaskViewModel()
        {
            SaveChangesCommand = new RelayCommand(SaveTask);
            EditChangesCommand = new RelayCommand(EditTask);
            CloseWindowCommand = new RelayCommand(CloseWindow);
        
            FillCategories();
        }

        private void SaveTask(object window)
        {
            string errors = Validate();
            if (!String.IsNullOrEmpty(errors)) { MessageBox.Show(errors); return; }


            try
            {
                Task t = new Task();

                t.Name = TaskName.Trim();
                t.Category = SelectedCategory;
                t.Category.Name = t.Category.Name.Trim();
                t.Deadline = Deadline;
                t.Description = TaskDescription.Trim();

                if (IsHighPriorityChecked) { t.PriorityID = 1; }
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
            finally
            {
                CloseWindow(window);
            }
        }
        private void EditTask(object window)
        {
            string errors = Validate();
            if (!string.IsNullOrEmpty(errors)) { MessageBox.Show(errors); return; }

            try
            {

                Task newValue = _dbContext.Tasks.Where(d => d.ID == OldTaskValue.ID).SingleOrDefault();
                newValue.Name = TaskName.Trim();
                if (IsHighPriorityChecked) { newValue.PriorityID = 1; }
                else if (IsMediumPriorityChecked) { newValue.PriorityID = 2; }
                else { newValue.PriorityID = 3; }
                newValue.Category = SelectedCategory;
                newValue.Category.Name = newValue.Category.Name.Trim();
                newValue.Deadline = Deadline;
                newValue.Description = TaskDescription.Trim();

                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            CloseWindow(window);
            
        }
        
        /// <summary>
        /// Validates all data in the window
        /// </summary>
        /// <returns>Empty string if validation passes and a string of the error if it does not.</returns>
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
        
        private void FillCategories()
        {
            var q = (from c in _dbContext.Categories select c).ToList();
            Categories = q;
        }
        private void CloseWindow(object o)
        {
            Window window = o as Window;
            if (window != null)
            {
                window.Close();
            }
        }
    
        #region Properties

        private Task oldTaskValue;
        public Task OldTaskValue
        {
            get { return oldTaskValue; }
            set { oldTaskValue = value; NotifyPropertyChanged("OldTaskValue"); }
        }

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
            set { taskName = value.Trim(); NotifyPropertyChanged("TaskName"); }
        }

        private string taskDescription;
        public string TaskDescription
        {
            get { return taskDescription; }
            set { taskDescription = value.Trim(); NotifyPropertyChanged("TaskDescription"); }
        }

        private Nullable<DateTime> deadline = null;
        public Nullable<DateTime> Deadline
        {
            get { return deadline; }
            set { deadline = value; NotifyPropertyChanged("Deadline"); }
        }
        
        #endregion

        public ICommand SaveChangesCommand { get; set; }
        public ICommand EditChangesCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }


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
