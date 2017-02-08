using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;

namespace TaskManagerApplication
{
    public class AddTaskViewModel : INotifyPropertyChanged
    {
        public AddTaskViewModel()
        {
            SaveChangesCommand = new RelayCommand(AddTask);

            FillCategories();
        }

        private TaskManagerDBContext _dbContext = new TaskManagerDBContext();

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

        private void FillCategories()
        {
            var q = (from c in _dbContext.Categories select c).ToList();
            Categories = q;
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

        public ICommand SaveChangesCommand { get; set; }

        private void AddTask(object o)
        {
           
            try
            {
                Task t = new Task();

                t.Name = TaskName;
                
                if (isHighPriorityChecked) { t.PriorityID = 1; }
                else if (IsMediumPriorityChecked) { t.PriorityID = 2; }
                else { t.PriorityID = 3; }

                t.Category = SelectedCategory;

                t.Deadline = Deadline;

                t.Description = TaskDescription;



                _dbContext.Tasks.Add(t);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }

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
