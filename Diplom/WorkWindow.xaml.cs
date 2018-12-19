﻿using Diplom.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {
		public List<int> numberStations;

        private IFocusable _focusedControl;
        public IFocusable FocusedControl
        {
            get { return _focusedControl; }

            set
            {
                if (value == null)
                {
                    RemoveMenuItem.IsEnabled = false;
                }
                else
                {
                    RemoveMenuItem.IsEnabled = true;
                }
                _focusedControl = value;
            }
        }

        public WorkWindow()
        {
            InitializeComponent();
			numberStations = new List<int>();
			Stock.workWindow = this;
		}

        public void CreateStation(string name = "")
        {
            var station = new StationControl(this, name);
            Canvas.SetLeft(station, 0);
            Canvas.SetTop(station, 0);
            foreach (Control control in canvas.Children)
            {
                if (control is IFocusable)
                {
                    IFocusable focusable = control as IFocusable;
                    station.FocusedElement += focusable.UnsetFocusBorder;
                    focusable.FocusedElement += station.UnsetFocusBorder;
                }
            }
            canvas.Children.Add(station);
            station.SetFocusBorder();
        }

        public void CreateManager(string name = "")
        {
            var manager = new ManagerControl(this, name);
            Canvas.SetLeft(manager, 0);
            Canvas.SetTop(manager, 0);
            foreach (Control control in canvas.Children)
            {
                if (control is IFocusable)
                {
                    IFocusable focusable = control as IFocusable;
                    manager.FocusedElement += focusable.UnsetFocusBorder;
                    focusable.FocusedElement += manager.UnsetFocusBorder;
                }
            }
            canvas.Children.Add(manager);
            manager.SetFocusBorder();
        }

        public void SetFocus(IFocusable control)
        {
            FocusedControl = control;
        }

        public void DropFocus()
        {
            FocusedControl?.UnsetFocusBorder();
            FocusedControl = null;
        }

        public void RemoveElement()
        {
            if (FocusedControl != null)
            {
                canvas.Children.Remove(FocusedControl as UserControl);
                foreach (IFocusable control in canvas.Children)
                {
                    control.FocusedElement -= FocusedControl.UnsetFocusBorder;
                }
                FocusedControl = null;
            }
        }

        private void canvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Station"))
            {
                var station = (StationControl)e.Data.GetData("Station");
                double shiftX = (double)e.Data.GetData("shiftX");
                double shiftY = (double)e.Data.GetData("shiftY");
                Canvas.SetLeft(station, e.GetPosition(canvas).X - shiftX);
                Canvas.SetTop(station, e.GetPosition(canvas).Y - shiftY);
            }
            else if (e.Data.GetDataPresent("Manager"))
            {
                var manager = (ManagerControl)e.Data.GetData("Manager");
                double shiftX = (double)e.Data.GetData("shiftX");
                double shiftY = (double)e.Data.GetData("shiftY");
                Canvas.SetLeft(manager, e.GetPosition(canvas).X - shiftX);
                Canvas.SetTop(manager, e.GetPosition(canvas).Y - shiftY);
            }
            e.Handled = true;
        }

        private void CreateNetwork_Click(object sender, RoutedEventArgs e)
        {
			if (numberStations.Count < 55)
			{
				ConfigurationNetwork wnd = new ConfigurationNetwork();
				wnd.Owner = this;
				wnd.Show();
			}
			else
				ShowErrorCountStations();
        }

        private void CreateStation_Click(object sender, RoutedEventArgs e)
        {
			if (numberStations.Count < 55)
			{
				ConfigurationStation wnd = new ConfigurationStation();
				wnd.Owner = this;
				wnd.Show();
			}
			else
				ShowErrorCountStations();
		}

		private void ShowErrorCountStations()
		{
			MessageBox.Show("Максимальное кол-во станций", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
		}

        private void CreateManager_Click(object sender, RoutedEventArgs e)
        {
            CreateManager();
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DropFocus();
            e.Handled = true;
        }

        private void RemoveElement_Click(object sender, RoutedEventArgs e)
        {
            RemoveElement();
        }

        public UserControl connector = null;
        public bool ConnectionAttempt(UserControl control)
        {
            if (connector == null) connector = control;
            else {
                if (connector is ManagerControl && control is ManagerControl) return false;

                //if ((connector is ManagerControl && control is StationControl) || (connector is StationControl && control is ManagerControl)) {
                SolidColorBrush brush;
                if (connector is StationControl && control is StationControl)
                {
                    brush = new SolidColorBrush(Colors.Green);

                    BitmapImage gauge = new BitmapImage();
                    gauge.BeginInit();
                    gauge.UriSource = new Uri("pack://application:,,,/Resources/gauge_1_30.png");
                    gauge.EndInit();
                    (connector as StationControl).stationGauge.Source = gauge;
                    (control as StationControl).stationGauge.Source = gauge;
                }
                else
                    brush = new SolidColorBrush(Colors.Blue);

                Line line = new Line();
                line.X1 = (double)connector.GetValue(Canvas.LeftProperty) + connector.ActualWidth / 2;
                line.Y1 = (double)connector.GetValue(Canvas.TopProperty) + connector.ActualHeight / 2;
                line.X2 = (double)control.GetValue(Canvas.LeftProperty) + connector.ActualWidth / 2;
                line.Y2 = (double)control.GetValue(Canvas.TopProperty) + connector.ActualHeight / 2;
                line.Stroke = brush;
                line.StrokeThickness = 3;
                Canvas.SetLeft(line, 0);
                Canvas.SetTop(line, 0);
                canvas.Children.Insert(0, line);
                connector = null;
                return true;
            }
            return false;
        }
    }
}
