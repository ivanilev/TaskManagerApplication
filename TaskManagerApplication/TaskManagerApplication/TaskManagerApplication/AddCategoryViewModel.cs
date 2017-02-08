using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace TaskManagerApplication
{
    public class AddCategoryViewModel : INotifyPropertyChanged
    {
        private TaskManagerDBContext _dbContext = new TaskManagerDBContext();

        public AddCategoryViewModel()
        {
            SaveChangesCommand = new RelayCommand(SaveCategory);
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
                categoryName = value;
                NotifyPropertyChanged("CategoryName");
            }
        }

        private void SaveCategory(object p)
        {
            try
            {
                Category C = new Category();
                C.Name = CategoryName;
                _dbContext.Categories.Add(C);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
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
