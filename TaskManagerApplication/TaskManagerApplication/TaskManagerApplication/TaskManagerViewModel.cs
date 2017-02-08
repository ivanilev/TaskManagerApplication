using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

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

            DispatcherTimerSetup();

            RefreshCommand = new RelayCommand(Refresh);
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

        #region User Notification functionality

        //Dispatcher Timer Setup
        private void DispatcherTimerSetup()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(10);
            dispatcherTimer.Tick += new EventHandler(PerformCheck);
            dispatcherTimer.Start();
        }

        private void PerformCheck(object sender, EventArgs e)
        {
            var q = (from t in ctx.Tasks orderby t.Deadline ascending select t).Take(1).ToArray();
            
            int days_Remaining = (q[0].Deadline.Value - DateTime.Now).Days; 
            if (days_Remaining < 3)
            {
                DeadlineComingUp = "Red";
            }
            else
            {
                DeadlineComingUp = "Transparent";
            }
        }

        private string deadlineComingUp;

        public string DeadlineComingUp
        {
            get { return deadlineComingUp; }
            set { deadlineComingUp = value; NotifyPropertyChanged("DeadlineComingUp"); }
        }
        #endregion

        #region Refresh Functionality

        private void Refresh(object o)
        {
            Tasks.Clear();
            FillAllTasks();

            Categories.Clear();
            FillCategories();

            TodaysTasks.Clear();
            FillTodaysTasks();
        }


        public ICommand RefreshCommand { get; set; }


        #endregion

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
