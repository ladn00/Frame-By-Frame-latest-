﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Фотостудия.Pages
{
    /// <summary>
    /// Логика взаимодействия для UslugiTable.xaml
    /// </summary>
    public partial class UslugiTable : Page
    {
        IEnumerable<Вид_услуги> currentList = MainWindow.db.Вид_услуги.ToList().OrderByDescending(p => p.Стоимость);

        public UslugiTable()
        {
            InitializeComponent();
            Refresh();
        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            var editButton = sender as Button;
            var selected = editButton.DataContext as Вид_услуги;
            AddVidUslugiWin edit = new AddVidUslugiWin(selected);
            currentList = MainWindow.db.Вид_услуги.ToList().OrderByDescending(p => p.Стоимость);
            edit.ShowDialog();
            Refresh();
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var deleted = lw1.SelectedItem as Вид_услуги;

                if (deleted != null)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Вы точно хотите удалить запись?",
                        "Внимание!",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error);

                    if (result == MessageBoxResult.Yes)
                    {
                        MainWindow.db.Вид_услуги.Remove(deleted);
                        MainWindow.db.SaveChanges();
                        MessageBox.Show("Запись удалена!");
                        currentList = MainWindow.db.Вид_услуги.ToList().OrderByDescending(p => p.Стоимость);
                        Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void VidAdd_Click(object sender, RoutedEventArgs e)
        {
            Вид_услуги vid = new Вид_услуги();
            vid.Номер_вида = 0;
            AddVidUslugiWin edit = new AddVidUslugiWin(vid);
            edit.ShowDialog();
            currentList = MainWindow.db.Вид_услуги.ToList().OrderByDescending(p => p.Стоимость);
            Refresh();
        }

        

        private void btFilterReset_Click(object sender, RoutedEventArgs e)
        {
            costFrom.Text = "";
            costTo.Text = "";
            tbSearch.Text = "";
            costByDesc.IsChecked = true;
            costBy.IsChecked = false;

            currentList = MainWindow.db.Вид_услуги.ToList().OrderByDescending(p => p.Стоимость);
            Refresh();
        }

        private void btFilterAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal from, to;

                if (!String.IsNullOrEmpty(costFrom.Text))
                {
                    if (!decimal.TryParse(costFrom.Text, out from) || !decimal.TryParse(costTo.Text, out to))
                        throw new Exception("Введите верное значение стоимости");

                    currentList = currentList.Where(p => p.Стоимость <= Convert.ToDecimal(costTo.Text) && p.Стоимость >= Convert.ToDecimal(costFrom.Text));
                }
                
                if(costBy.IsChecked == true)
                {
                    currentList = currentList.OrderBy(p => p.Стоимость);
                }
                else
                {
                    currentList = currentList.OrderByDescending(p => p.Стоимость);
                }

                lw1.ItemsSource = currentList;
                Refresh();
                spFilter.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void searchTextChanged(object sender, TextChangedEventArgs e)
        {
            string seachText = tbSearch.Text;
            var newlist = currentList;
            if (!string.IsNullOrWhiteSpace(seachText))
            {
                newlist = currentList.Where(p => p.Название.ToLower().Contains(seachText.ToLower())).ToList();
            }

            _maxPages = (int)Math.Ceiling(currentList.Count() * 1.0 / _count);

            var listPage = newlist.Skip((_currentPage - 1) * _count).Take(_count).ToList();

            TxtCurrentPage.Text = _currentPage.ToString();
            LblTotalPages.Content = "из " + _maxPages;

            lw1.ItemsSource = listPage;
        }

        private void bt_filter(object sender, RoutedEventArgs e)
        {
            if (spFilter.Visibility != Visibility.Visible)
                spFilter.Visibility = Visibility.Visible;
            else
                spFilter.Visibility = Visibility.Hidden;
        }

        private int _currentPage = 1;
        private int _count = 6;
        private int _maxPages;

        private void Refresh()
        {
            _maxPages = (int)Math.Ceiling(currentList.Count() * 1.0 / _count);

            var listPage = currentList.Skip((_currentPage - 1) * _count).Take(_count).ToList();

            TxtCurrentPage.Text = _currentPage.ToString();
            LblTotalPages.Content = "из " + _maxPages;

            lw1.ItemsSource = listPage;
        }

        private void GoToFirstPage(object sender, RoutedEventArgs e)
        {
            _currentPage = 1;
            Refresh();
        }

        private void GoToPreviousPage(object sender, RoutedEventArgs e)
        {
            if (_currentPage <= 1) _currentPage = 1;
            else
                _currentPage--;
            Refresh();
        }

        private void GoToNextPage(object sender, RoutedEventArgs e)
        {
            if (_currentPage >= _maxPages) _currentPage = _maxPages;
            else
                _currentPage++;
            Refresh();
        }

        private void GoToLastPage(object sender, RoutedEventArgs e)
        {
            _currentPage = _maxPages;
            Refresh();
        }
    }
}
