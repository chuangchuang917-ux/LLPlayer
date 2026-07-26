using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using LLPlayer.Services;
using Prism.Services.Dialogs;

namespace LLPlayer.Views;

public partial class VocabularyDialog : UserControl, IDialogAware
{
    private readonly VocabularyService _vocabService;
    private readonly AnkiConnectService _ankiService;

    public event Action<IDialogResult>? RequestClose;

    public string Title => "生字簿與 Anki 管理";

    public VocabularyDialog()
    {
        InitializeComponent();

        _vocabService = ((App)Application.Current).Container.Resolve<VocabularyService>();
        _ankiService = ((App)Application.Current).Container.Resolve<AnkiConnectService>();

        RefreshList();
    }

    private void RefreshList()
    {
        string query = SearchBox.Text.Trim().ToLower();
        var items = _vocabService.Items.AsEnumerable();

        if (!string.IsNullOrEmpty(query))
        {
            items = items.Where(i =>
                i.Word.ToLower().Contains(query) ||
                i.Definition.ToLower().Contains(query) ||
                i.ContextSentence.ToLower().Contains(query)
            );
        }

        var list = items.ToList();
        VocabDataGrid.ItemsSource = list;
        StatusText.Text = $"總計 {list.Count} 個生字";
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        RefreshList();
    }

    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is VocabularyItem item)
        {
            _vocabService.Remove(item);
            RefreshList();
        }
    }

    private void ExportCsv_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "CSV 檔案 (*.csv)|*.csv",
            FileName = $"LLPlayer_Vocabulary_{DateTime.Now:yyyyMMdd}.csv"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                _vocabService.ExportToCsv(dialog.FileName);
                MessageBox.Show("生字檔已成功匯出！", "匯出成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"匯出失敗: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void SyncAllAnki_Click(object sender, RoutedEventArgs e)
    {
        bool available = await _ankiService.IsAvailableAsync();
        if (!available)
        {
            MessageBox.Show("未能在本機檢測到 AnkiConnect 服務 (127.0.0.1:8765)。\n請確認 Anki 已開啟並安裝 AnkiConnect 外掛。", "連線失敗", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int successCount = 0;
        int failCount = 0;

        foreach (var item in _vocabService.Items)
        {
            var (success, _) = await _ankiService.AddNoteAsync(item.Word, item.Definition, item.ContextSentence, item.VideoTitle);
            if (success) successCount++;
            else failCount++;
        }

        MessageBox.Show($"Anki 同步完成！\n成功: {successCount} 張, 失敗/重複: {failCount} 張", "Anki 同步結果", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
    }

    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
    public void OnDialogOpened(IDialogParameters parameters) { }
}
