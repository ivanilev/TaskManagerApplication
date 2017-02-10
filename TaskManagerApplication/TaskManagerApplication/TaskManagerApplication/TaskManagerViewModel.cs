using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
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


            AddCategoryCommand = new RelayCommand(AddCategory);
            DeleteCategoryCommand = new RelayCommand(DeleteCategory);
            EditCategoryCommand = new RelayCommand(EditCategory);

            AddTaskCommand = new RelayCommand(AddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
            EditTaskCommand = new RelayCommand(EditTask);
            CompleteTaskCommand = new RelayCommand(CompleteTask);

            RefreshCommand = new RelayCommand(Refresh);

        }

        private void FillAllTasks()
        {
            var query = (from t in ctx.Tasks select t).ToList();
            query = query.Where(t => t.IsComplete == false).ToList();
            this.Tasks = new ObservableCollection<Task>(query);
        }
        private void FillCategories()
        {
            var query = (from c in ctx.Categories select c).ToList();
            this.Categories = new ObservableCollection<Category>(query);
        }
        private void FillTodaysTasks()
        {
            var query = (
                from t in ctx.Tasks
                join p in ctx.Priorities on t.PriorityID equals p.ID
                orderby p.ID
                select t).Take(10).ToList();

            query = query.Where(t => t.IsComplete == false).ToList();
            this.TodaysTasks = new ObservableCollection<Task>(query);
        }

        private ObservableCollection<TaskManagerApplication.Task> todaysTasks;
        public ObservableCollection<TaskManagerApplication.Task> TodaysTasks
        {
            get { return todaysTasks; }
            set { todaysTasks = value; NotifyPropertyChanged("TodaysTasks"); }
        }

        #region Categories

        private Category selectedCategoryItem;
        public Category SelectedCategoryItem
        {
            get { return selectedCategoryItem; }
            set { selectedCategoryItem = value; NotifyPropertyChanged("SelectedCategoryItem"); }
        }

        private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set { categories = value; NotifyPropertyChanged("Categories"); }
        }

        public ICommand AddCategoryCommand { get; set; }
        public ICommand DeleteCategoryCommand { get; set; }
        public ICommand EditCategoryCommand { get; set; }

        private void AddCategory(object o)
        {
            var categoryWindow = new AddCategoryWindow();
            categoryWindow.Show();
        }
        private void DeleteCategory(object o)
        {
            try
            {
                ctx.Categories.Remove(SelectedCategoryItem);
                ctx.SaveChanges();
                Refresh(null);
            }
            catch(InvalidOperationException e)
            {
                MessageBox.Show("Cannot delete selected category item because there are tasks dependent on it. Please try deleting said tasks first and try again.");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }
        private void EditCategory(object o)
        {
            EditCategoryWindow editWindow = new EditCategoryWindow();

            editWindow.DataContext = new CategoryViewModel();

            ((CategoryViewModel)editWindow.DataContext).CategoryName = SelectedCategoryItem.Name;

            ((CategoryViewModel)editWindow.DataContext).OldValue = SelectedCategoryItem;

            editWindow.Show();

        }

        #endregion

        #region AllTasks

        private Task allTasksSelectedItem;
        public Task AllTasksSelectedItem
        {
            get { return allTasksSelectedItem; }
            set { allTasksSelectedItem = value; NotifyPropertyChanged("AllTasksSelectedItem"); }
        }

        private ObservableCollection<TaskManagerApplication.Task> tasks;
        public ObservableCollection<TaskManagerApplication.Task> Tasks
        {
            get { return tasks; }
            set { tasks = value; NotifyPropertyChanged("Tasks"); }
        }

        public ICommand AddTaskCommand { get; set; }
        public ICommand DeleteTaskCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }
        public ICommand CompleteTaskCommand { get; set; }

        private void AddTask(object o)
        {
            var taskWindow = new AddTaskWindow();
            taskWindow.Show();
        }
        private void DeleteTask(object o)
        {
            try
            {
                ctx.Tasks.Remove(AllTasksSelectedItem);
                ctx.SaveChanges();
                Refresh(null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void EditTask(object o)
        {
            EditTaskWindow editWindow = new EditTaskWindow();

            editWindow.DataContext = new TaskViewModel();

            ((TaskViewModel)editWindow.DataContext).TaskName = AllTasksSelectedItem.Name;

            //I have a feeling there's a smarter way to do this
            if(AllTasksSelectedItem.Priority.Setting.Trim()=="High")
            {
                ((TaskViewModel)editWindow.DataContext).IsHighPriorityChecked = true;
                ((TaskViewModel)editWindow.DataContext).IsMediumPriorityChecked = false;
                ((TaskViewModel)editWindow.DataContext).IsLowPriorityChecked = false;
            }
            else if (AllTasksSelectedItem.Priority.Setting.Trim() == "Medium")
            {
                ((TaskViewModel)editWindow.DataContext).IsHighPriorityChecked = false;
                ((TaskViewModel)editWindow.DataContext).IsMediumPriorityChecked = true;
                ((TaskViewModel)editWindow.DataContext).IsLowPriorityChecked = false;
            }
            else
            {
                ((TaskViewModel)editWindow.DataContext).IsHighPriorityChecked = false;
                ((TaskViewModel)editWindow.DataContext).IsMediumPriorityChecked = false;
                ((TaskViewModel)editWindow.DataContext).IsLowPriorityChecked = true;
            }

            ((TaskViewModel)editWindow.DataContext).Deadline = AllTasksSelectedItem.Deadline;
            ((TaskViewModel)editWindow.DataContext).SelectedCategory = AllTasksSelectedItem.Category;
            ((TaskViewModel)editWindow.DataContext).TaskDescription = AllTasksSelectedItem.Description;

            ((TaskViewModel)editWindow.DataContext).OldTaskValue = AllTasksSelectedItem;
            
            editWindow.Show();
        }
        private void CompleteTask(object o)
        {
            try
            {
                ctx.Tasks.FirstOrDefault(x => x.ID == AllTasksSelectedItem.ID).IsComplete = true;
                ctx.SaveChanges();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Refresh(null);
            }
        }

        #endregion

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
