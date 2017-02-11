using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;

namespace TaskManagerApplication
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        private TaskManagerDBContext _dbContext = new TaskManagerDBContext();

        public CategoryViewModel()
        {
            SaveChangesCommand = new RelayCommand(SaveCategory);
            EditCategoryCommand = new RelayCommand(EditCategory);
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        private bool Validate()
        {
            var categories = (from c in _dbContext.Categories select c).ToList();

            return (string.IsNullOrEmpty(CategoryName) == false
                && categories.Where(x => x.Name == CategoryName).ToList().Count == 0);
        }

        private string categoryName;
        public string CategoryName
        {
            get
            {
                return categoryName;
            }
            set
            {
                categoryName = value.Trim();
                NotifyPropertyChanged("CategoryName");
            }
        }

        private Category oldValue;
        public Category OldValue
        {
            get { return oldValue; }
            set { oldValue = value; NotifyPropertyChanged("OldValue"); }
        }

        private void EditCategory(object p)
        {

            if (!Validate()) { MessageBox.Show("Error, validation failed!"); return; }

            try
            {
                _dbContext.Categories.Where(z => z.Name == OldValue.Name).FirstOrDefault().Name = CategoryName.Trim();
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            CloseWindow(p);
            
        }
        private void SaveCategory(object p)
        {
            if (!Validate()) { MessageBox.Show("Error, validation failed!"); return; }
            try
            {
                Category C = new Category();
                C.Name = CategoryName.Trim();
                _dbContext.Categories.Add(C);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                CloseWindow(p);
            }
        }
        private void CloseWindow(object o)
        {
            Window window = o as Window;
            if (window != null)
            {
                window.Close();
            }
        }
    
        public ICommand SaveChangesCommand { get; set; }
        public ICommand EditCategoryCommand { get; set; }
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
