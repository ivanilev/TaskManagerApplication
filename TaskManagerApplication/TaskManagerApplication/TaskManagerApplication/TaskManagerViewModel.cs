using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TaskManagerApplication
{
    public class TaskManagerViewModel : INotifyPropertyChanged
    {
        TaskManagerDBContext ctx = new TaskManagerDBContext();

        public TaskManagerViewModel()
        {
            FillAllTasks();
            FillCategories();
            FillTodaysTasks();
        }

        private void FillAllTasks()
        {
            var query = (from t in ctx.Tasks select t).ToList();
            this.Tasks = query;
        }
        private void FillCategories()
        {
            var query = (from c in ctx.Categories select c).ToList();
            this.Categories = query;
        }
        private void FillTodaysTasks()
        {
            var query = (
                from t in ctx.Tasks
                join p in ctx.Priorities on t.PriorityID equals p.ID
                orderby p.ID
                select t).Take(5).ToList();

            this.TodaysTasks = query;
        }

        private List<TaskManagerApplication.Task> tasks;
        public List<TaskManagerApplication.Task> Tasks
        {
            get { return tasks; }
            set { tasks = value; NotifyPropertyChanged("Tasks"); }
        }

        private List<Category> categories;
        public List<Category> Categories
        {
            get { return categories; }
            set { categories = value; NotifyPropertyChanged("Categories"); }
        }

        private List<TaskManagerApplication.Task> todaysTasks;
        public List<TaskManagerApplication.Task> TodaysTasks
        {
            get { return todaysTasks; }
            set { todaysTasks = value; NotifyPropertyChanged("TodaysTasks"); }
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
