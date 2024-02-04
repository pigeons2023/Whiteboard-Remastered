using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using CsvHelper;
using System.Globalization;
using Microsoft.Win32;

namespace LotteryApp
{
    public partial class LotteryWindow : Window
    {
        private List<Participant> participants = new List<Participant>();

        public LotteryWindow()
        {
            InitializeComponent();
        }

        private void DrawOne_Click(object sender, RoutedEventArgs e)
        {
            var winners = DrawWinners(1);
            ShowWinners(winners);
        }

        private void DrawMultiple_Click(object sender, RoutedEventArgs e)
        {
            var winners = DrawWinners(3);
            ShowWinners(winners);
        }

        private void SelectParticipantsFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ReadParticipantsFromCsv(openFileDialog.FileName);
            }
        }

        private void ReadParticipantsFromCsv(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    participants = csv.GetRecords<Participant>().ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read CSV file: {ex.Message}");
            }
        }

        private List<Participant> DrawWinners(int numWinners)
        {
            var rnd = new Random();
            var winners = new List<Participant>();
            var weightedList = participants.SelectMany(p => Enumerable.Repeat(p, p.Weight)).ToList();

            while (winners.Count < numWinners && weightedList.Any())
            {
                int index = rnd.Next(weightedList.Count);
                winners.Add(weightedList[index]);
                weightedList.RemoveAll(x => x.Name == weightedList[index].Name); // Ensure unique winners
            }

            return winners;
        }

        private void ShowWinners(List<Participant> winners)
        {
            txtWinners.Text = "获奖者:\n" + string.Join("\n", winners.Select(w => w.Name));
            txtWinners.FontWeight = FontWeights.Bold; // 设置加粗
            txtWinners.FontSize = 24; // 设置字号
        }
    }

    public class Participant
    {
        public string Name { get; set; }
        public int Weight { get; set; } // Weight for lottery draw
    }
}
