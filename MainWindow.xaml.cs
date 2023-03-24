using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FibonacciCalculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {

            try { 
                int limit = int.Parse(txtLimit.Text);

                // Create a new progress object to report progress to the UI
                Progress<int> progress = new Progress<int>(percentComplete =>
                {
                    // Update the progress bar on the UI thread
                    pbProgress.Value = percentComplete;
                });
                // Disable the button and show the progress bar
                btnCalculate.IsEnabled = false;
                pbProgress.Visibility = Visibility.Visible;
                // Calculate the Fibonacci numbers asynchronously
                List<BigInteger> fibonacciNumbers = await Task.Run(() => CalculateFibonacciNumbers(limit, progress));
                // Update the UI with the results on the main UI thread
                    Dispatcher.Invoke(() =>
                    {
                        // Enable the button and hide the progress bar
                        btnCalculate.IsEnabled = true;
                        pbProgress.Visibility = Visibility.Hidden;

                        // Display the Fibonacci numbers in the ListBox
                        lstFibonacciNumbers.ItemsSource = fibonacciNumbers;
                    });
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("The operation was canceled.");
                }
                catch (Exception ex)
                {
                //if user inputed date not correctly all the text box will be empty and user will see message
                lstFibonacciNumbers.ItemsSource = null;
                txtLimit.Text = string.Empty;
                MessageBox.Show($"An error occurred: {ex.Message}");
                }

        }

        private static List<BigInteger> CalculateFibonacciNumbers(int limit, IProgress<int> progress)
        {
            List<BigInteger> fibonacciNumbers = new List<BigInteger>();

            // Calculate the Fibonacci numbers
            BigInteger a = 1;
            BigInteger b = 1;

            for (int i = 0; i < limit; i++)
            {
                BigInteger temp = a;
                a = b;
                b = temp + b;


                fibonacciNumbers.Add(a);

                // Report progress
                if (progress != null)
                {
                    int percentComplete = (int)Math.Round((double)(i + 1) / limit * 100);
                    progress.Report(percentComplete);
                }

                // Check for cancellation
                if (Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
                {
                    return null;
                }
            }

            return fibonacciNumbers;
        }
    }
}