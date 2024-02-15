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
		public RelayCommand? OpenTask0Command {get; private set; }
		public RelayCommand? OpenTask1Command { get; private set; }
		public RelayCommand? OpenTask2Command { get; private set; }
		public RelayCommand? OpenTask3Command { get; private set; }

		// View
		private object? _currentView;
		public object? CurrentView
		{
			get => _currentView;
			private set
			{
				_currentView = value;
				OnPropertyChanged();
			}
		}

		public MainWindowViewModel()
		{
			var task1View = new Task1View();
			var task2View = new Task2View();

			OpenTask0Command = new RelayCommand(_ => CurrentView = null);
			OpenTask1Command = new RelayCommand(_ => CurrentView = task1View);
			OpenTask2Command = new RelayCommand(_ => CurrentView = task2View);
			OpenTask3Command = new RelayCommand(_ => throw new NotImplementedException("Task3 is not 'yet' implemented"));
		}
	}
}
