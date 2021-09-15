﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.UI;
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Printing;
using Windows.UI.Core;
using System.Windows;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.ApplicationModel.Activation;
// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace WIn11
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    /// 



    public sealed partial class MainPage : Page
    {
        bool changed;

        public MainPage()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            this.InitializeComponent();

            var view = ApplicationView.GetForCurrentView();
            
            Color cl = txt.Document.Selection.CharacterFormat.ForegroundColor;
            
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            changed = false;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += async (sender, args) =>
            {
                args.Handled = true;

                var curView = CoreApplication.GetCurrentView();
                var newWindow = curView.CoreWindow;


                if (changed == true)
                {
                    try
                    {
                        ContentDialog saveDialog = new ContentDialog
                        {
                            Title = resourceLoader.GetString("ExitSav"),
                            Content = resourceLoader.GetString("ExitDesc"),
                            PrimaryButtonText = resourceLoader.GetString("SaveExitFile"),
                            SecondaryButtonText = resourceLoader.GetString("Exit"),
                            CloseButtonText = resourceLoader.GetString("Stop"),
                            DefaultButton = ContentDialogButton.Primary
                        };
                        var result = await saveDialog.ShowAsync();
                        if (result == ContentDialogResult.Secondary)
                        {
                            Application.Current.Exit();
                        }
                        if (result == ContentDialogResult.Primary)
                        {
                            SaveExit();
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    Application.Current.Exit();
                }

            };


            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;
        }



        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }



        // Update the TitleBar based on the inactive/active state of the app
        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitle.Foreground = inactiveForegroundBrush;


            }
            else
            {
                AppTitle.Foreground = defaultForegroundBrush;


            }
        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            // If the back button is not visible, reduce the TitleBar content indent.


            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (sender.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }



        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            txt.TextDocument.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, "");

            Edit.Opacity = 0;
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.Title = "New Document";
            changed = false;
        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            Windows.Storage.Pickers.FileOpenPicker open =
                new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".txt");
            open.FileTypeFilter.Add(".rtf");

            Windows.Storage.StorageFile file = await open.PickSingleFileAsync();

            if (file != null)
            {
                try
                {
                    IBuffer buffer = await FileIO.ReadBufferAsync(file);
                    var reader = DataReader.FromBuffer(buffer);
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string text = reader.ReadString(buffer.Length);
                    appView.Title = file.Name;
                    txt.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, text);
                    changed = false;
                }
                catch (Exception)
                {
                    ContentDialog errorDialog = new ContentDialog()
                    {
                        Title = resourceLoader.GetString("FileError"),
                        Content = resourceLoader.GetString("FileDesc"),
                        PrimaryButtonText = "Ok",
                        DefaultButton = ContentDialogButton.Primary
                    };
                    await errorDialog.ShowAsync();
                }
            }

            else
            {

                appView.Title = "New Document";
            }
        }

        private async void Save()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;


            
            savePicker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
            savePicker.FileTypeChoices.Add("Rich Text Document", new List<string>() { ".rtf" });

            savePicker.SuggestedFileName = "New Document";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    txt.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                }


                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status != FileUpdateStatus.Complete)
                {
                    Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                    await errorBox.ShowAsync();
                }
            }

            changed = false;
            if (changed == true)
            {
                Edit.Opacity = 0.6;
            }
            else
            {
                Edit.Opacity = 0;
            }
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            if (file != null)
            {
                appView.Title = file.Name;
            }
            else
            {
                appView.Title = "New Document";
            }
        }

        private async void SaveExit()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;


            savePicker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
            savePicker.FileTypeChoices.Add("Rich Text Document", new List<string>() { ".rtf" });

            savePicker.SuggestedFileName = "New Document";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {

                CachedFileManager.DeferUpdates(file);

                using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    txt.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                }


                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status != FileUpdateStatus.Complete)
                {
                    Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                    await errorBox.ShowAsync();
                }
            }


            Application.Current.Exit();

        }

        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            Save();
        }

        public static async Task<string> ShowAddDialogAsync(string title)
        {
            var inputTextBox = new TextBox { AcceptsReturn = false };
            (inputTextBox as FrameworkElement).VerticalAlignment = VerticalAlignment.Bottom;
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            var dialog = new ContentDialog
            {
                Content = inputTextBox,
                Title = title,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Ok",
                SecondaryButtonText = resourceLoader.GetString("Stop"),
                DefaultButton = ContentDialogButton.Primary
            };
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)

                return inputTextBox.Text;
            else
                return "u";
        }

        private async void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                txt.FontFamily = new FontFamily(await ShowAddDialogAsync("Font"));
            }
            catch
            {

            }
        }

        private async void MenuFlyoutItem_Click_4(object sender, RoutedEventArgs e)
        {
            ContentDialog AboutDialog = new ContentDialog
            {
                Title = "Notes",
                Content = "PreRelease 0.5.4",
                CloseButtonText = "Ok!",
                DefaultButton = ContentDialogButton.Close
            };

            var result = await AboutDialog.ShowAsync();
        }

        private void MenuFlyoutItem_Click_5(object sender, RoutedEventArgs e)
        {
            txt.FontSize = (txt.FontSize + 1);
        }

        private void MenuFlyoutItem_Click_6(object sender, RoutedEventArgs e)
        {
            txt.FontSize = (txt.FontSize - 1);
        }

        private async void compactOverlayButton_Click(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            compactOptions.CustomSize = new Windows.Foundation.Size(320, 330);
            bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions);
            exitbutt.Visibility = Visibility.Visible;
            menuBar.Visibility = Visibility.Collapsed;
            AppFontIcon.Visibility = Visibility.Collapsed;
            AppTitle.HorizontalAlignment = HorizontalAlignment.Center;
            Thickness margin = AppTitle.Margin;
            margin.Left = 40;
            AppTitle.Margin = margin;
            Thickness marginEdit = Edit.Margin;
            Edit.Margin = marginEdit;
            marginEdit.Left = 33;
            menuBar.HorizontalAlignment = HorizontalAlignment.Left;
            Thickness marginMenu = menuBar.Margin;
            marginMenu.Top = 41;
            menuBar.Margin = marginMenu;
            ful.Text = resourceLoader.GetString("FullScreen/Text");
            ful.Icon = new SymbolIcon(Symbol.FullScreen);
        }
        private async void standardModeButton_Click(object sender, RoutedEventArgs e)
        {
            bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
            exitbutt.Visibility = Visibility.Collapsed;
            menuBar.Visibility = Visibility.Visible;
            Thickness margin = AppTitle.Margin;
            Thickness marginEdit = Edit.Margin;
            margin.Left = 15;
            marginEdit.Left = 8;
            AppTitle.Margin = margin;
            Edit.Margin = marginEdit;
            AppFontIcon.Visibility = Visibility.Visible;
        }

        



        public async Task<string> GetFileText(string filePath)
        {
            var stringContent = "";
            
                var file = await StorageFile.GetFileFromPathAsync(filePath);

                if (file != null)
                {
                    stringContent = await Windows.Storage.FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                }
            
            

            return stringContent;
        }

        bool printingAct = false;

        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;

        #region Register for printing

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            if (printingAct == true)
            {
                printMan = PrintManager.GetForCurrentView();
                printMan.PrintTaskRequested += PrintTaskRequested;

                printDoc = new PrintDocument();
                printDocSource = printDoc.DocumentSource;
                printDoc.Paginate += Paginate;
                printDoc.GetPreviewPage += GetPreviewPage;
                printDoc.AddPages += AddPages;
            }
            else
            {
                base.OnNavigatedTo(e);
                var args = e.Parameter as FileActivatedEventArgs;

                if (args != null)
                {
                    if (args.Kind == Windows.ApplicationModel.Activation.ActivationKind.File)
                    {
                        string strFileName = args.Files[0].Name;
                        string strFile = args.Files[0].Path;
                        if (args.Files != null)
                        {
                            changed = false;
                            try
                            {
                                var text = await GetFileText(@strFile);

                                var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
                                appView.Title = strFileName;
                                txt.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, text);
                                
                            }
                            catch
                            {
                                ContentDialog fileError = new ContentDialog()
                                {
                                    Title = "Oops...",
                                    Content = resourceLoader.GetString("FileDesc"),
                                    PrimaryButtonText = "OK"
                                };
                                await fileError.ShowAsync();
                            }
                            
                        }
                    }
                }
            }
        }

        #endregion

        #region Showing the print dialog

        private void PrintButton(object sender, RoutedEventArgs e)
        {
            printingAct = true;
            PrintButtonClick();
        }
    private async void PrintButtonClick()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            if (PrintManager.IsSupported())
            {
                try
                {
                    await PrintManager.ShowPrintUIAsync();
                    txt.Foreground = new SolidColorBrush(Colors.Black);
                }
                catch
                {
                    
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "Printing Is Currently Unavailable...",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                }

            }
            else
            {
                ContentDialog noPrintingDialog = new ContentDialog()
                {
                    Title = "Printing...",
                    Content = resourceLoader.GetString("PrintError"),
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }
        }

        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequrested);
            {
                IList<string> displayedOptions = printTask.Options.DisplayedOptions;
            }
            
            printTask.Completed += PrintTaskCompleted;


        }

        private void PrintTaskSourceRequrested(PrintTaskSourceRequestedArgs args)
        {
            args.SetSource(printDocSource);
        }

        #endregion

        #region Print preview

        private void Paginate(object sender, PaginateEventArgs e)
        {
            printDoc.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            printDoc.SetPreviewPage(e.PageNumber, txt);
        }

        #endregion

        #region Add pages to send to the printer

        private void AddPages(object sender, AddPagesEventArgs e)
        {
            printDoc.AddPage(txt);

            printDoc.AddPagesComplete();
        }

        #endregion

        #region Print task completed



        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing...",
                        Content = resourceLoader.GetString("FileDesc"),
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                });
            }
            
        }

        #endregion

        private void MenuFlyoutItem_Click_7(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            if (AudioTg.IsChecked)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
                AudioTg.Icon = new SymbolIcon(Symbol.Audio);
                AudioTg.Text = resourceLoader.GetString("AudioActivated/Text");
            }
            else
            {
                AudioTg.Icon = new SymbolIcon(Symbol.Mute);
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
                AudioTg.Text = resourceLoader.GetString("AudioDeactivated/Text");
            }
            
        }

        private void MenuFlyoutItem_Click_8(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                ful.Text = resourceLoader.GetString("FullScreen/Text");
                ful.Icon = new SymbolIcon(Symbol.FullScreen);
                menuBar.HorizontalAlignment = HorizontalAlignment.Left;
                Thickness margin = menuBar.Margin;
                margin.Top = 41;
                menuBar.Margin = margin;
            }
            else
            {
                view.TryEnterFullScreenMode();
                ful.Text = resourceLoader.GetString("Window/Text");
                ful.Icon = new SymbolIcon(Symbol.BackToWindow);
                menuBar.HorizontalAlignment = HorizontalAlignment.Center;
                Thickness margin = menuBar.Margin;
                margin.Top = 7;
                menuBar.Margin = margin;
            }
        }

        private void MenuFlyoutItem_Click_9(object sender, RoutedEventArgs e)
        {
            FindBoxHighlightMatches();
        }

        private async void FindBoxHighlightMatches()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            var inputTextBox = new TextBox { AcceptsReturn = false };
            (inputTextBox as FrameworkElement).VerticalAlignment = VerticalAlignment.Bottom;
            var dialog = new ContentDialog
            {
                Content = inputTextBox,
                Title = resourceLoader.GetString("SearchQuest"),
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = resourceLoader.GetString("SearchButton"),
                SecondaryButtonText = resourceLoader.GetString("Stop"),
                DefaultButton = ContentDialogButton.Primary
            };
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                FindBoxRemoveHighlights();

                Color highlightBackgroundColor = (Color)App.Current.Resources["SystemColorHighlightColor"];
                Color highlightForegroundColor = (Color)App.Current.Resources["SystemColorHighlightTextColor"];

                string textToFind = inputTextBox.Text;
                if (textToFind != null)
                {
                    ITextRange searchRange = txt.Document.GetRange(0, 0);
                    while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
                    {
                        searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                        searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
                    }
                }
            }

            else
            {

            }


            }

        private void FindBoxRemoveHighlights()
        {
            ITextRange documentRange = txt.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush defaultBackground = txt.Background as SolidColorBrush;
            SolidColorBrush defaultForeground = txt.Foreground as SolidColorBrush;

            documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
            documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
        }

        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            txt.Document.GetText(TextGetOptions.UseCrlf, out string currentRawText);

            // reset colors to correct defaults for Focused state
            ITextRange documentRange = txt.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush background = new SolidColorBrush(Colors.Transparent);
            SolidColorBrush defaultForeground = txt.Foreground as SolidColorBrush;
            

            if (background != null)
            {
                documentRange.CharacterFormat.BackgroundColor = background.Color;
                if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    txt.Foreground = new SolidColorBrush(Colors.White);
                    txt.Document.Selection.CharacterFormat.ForegroundColor = Colors.White;
                }
                else
                {
                    txt.Foreground = new SolidColorBrush(Colors.Black);
                    txt.Document.Selection.CharacterFormat.ForegroundColor = Colors.Black;
                }
            }
        }

        private void txt_TextChanged(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            var view = ApplicationView.GetForCurrentView();
            RichEditBox richemp = new RichEditBox();
            printingAct = false;
            if (txt.Document == richemp.Document)
            {
                changed = false;
            }
            else
            {
                changed = true;
            }
            if (changed == true)
            {
                Edit.Opacity = 0.6;
            }
            else
            {

                Edit.Opacity = 0;
            }
            if (view.IsFullScreenMode)
            {
                ful.Text = resourceLoader.GetString("Window/Text");
                ful.Icon = new SymbolIcon(Symbol.BackToWindow);
                menuBar.HorizontalAlignment = HorizontalAlignment.Center;
                Thickness margin = menuBar.Margin;
                margin.Top = 7;
                menuBar.Margin = margin;
            }
            else
            {
                ful.Text = resourceLoader.GetString("FullScreen/Text");
                ful.Icon = new SymbolIcon(Symbol.FullScreen);
                menuBar.HorizontalAlignment = HorizontalAlignment.Left;
                Thickness margin = menuBar.Margin;
                margin.Top = 41;
                menuBar.Margin = margin;
            }


        }
            
    }
    
}