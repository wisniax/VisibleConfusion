using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibleConfusion.Core;
using VisibleConfusion.MVVM.View;

namespace VisibleConfusion.MVVM.ViewModel
{
	internal class MainWindowViewModel : ObservableObject
	{
		public RelayCommand? OpenTask1Command { get; private set; }
		public RelayCommand? OpenTask2Command { get; private set; }
		public RelayCommand? OpenTask3Command { get; private set; }

		// View
		private object _currentView;
		public object CurrentView
		{
			get => _currentView;
			private set
			{
				_currentView = value;
				OnPropertyChanged();
			}
		}

		Task1View _task1ViewView;

		public MainWindowViewModel()
		{
			
		}
	}
}
