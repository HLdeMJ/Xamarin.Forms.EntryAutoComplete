﻿using System.Collections;
using System.Linq;
using Xamarin.Forms;

namespace EntryAutoComplete.CustomControl
{
    public enum Mode
    {
        StartsWith,
        Contains,
        EndsWith
    }

    public class EntryAutoComplete : ContentView
    {
        private const int RowHeight = 40;

        public static readonly BindableProperty SearchTextProperty =
            BindableProperty.Create(nameof(SearchText), typeof(string), typeof(EntryAutoComplete),
                defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSearchTextChanged);

        public static readonly BindableProperty SearchTextColorProperty =
            BindableProperty.Create(nameof(SearchTextColor), typeof(Color), typeof(EntryAutoComplete), Color.Black,
                propertyChanged: OnSearchTextColorChanged);

        public static readonly BindableProperty MaximumVisibleElementsProperty =
            BindableProperty.Create(nameof(MaximumVisibleElements), typeof(int), typeof(EntryAutoComplete), 4);

        public static readonly BindableProperty MinimumPrefixCharacterProperty =
            BindableProperty.Create(nameof(MinimumPrefixCharacter), typeof(int), typeof(EntryAutoComplete), 1);

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(EntryAutoComplete),
                propertyChanged: OnPlaceholderChanged);

        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(EntryAutoComplete), Color.DarkGray,
                propertyChanged: OnPlaceholderColorChanged);

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(EntryAutoComplete));

        public static readonly BindableProperty IsClearButtonVisibleProperty =
            BindableProperty.Create(nameof(IsClearButtonVisible), typeof(bool), typeof(EntryAutoComplete), true,
                propertyChanged: OnIsClearImageVisibleChanged);

        public static readonly BindableProperty SearchTypeProperty = BindableProperty.Create(nameof(SearchType),
            typeof(Mode), typeof(Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry),
            Mode.Contains, BindingMode.OneWay, null, propertyChanged: OnSearchTypeChanged);

        private static void OnSearchTextChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var autoCompleteView = bindable as EntryAutoComplete;
            var searchText = (string) newvalue;
            autoCompleteView.SearchText = searchText;
            autoCompleteView.SuggestionsStackLayout.Children.Clear();
            foreach (var item in autoCompleteView.ItemsSource)
            {
                autoCompleteView.SuggestionsStackLayout.Children.Add(new Label { Text = item.ToString() });
            }
            autoCompleteView._originSuggestions = autoCompleteView.ItemsSource;
        }

        private static void OnSearchTextColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var entryAutoComplete = bindable as EntryAutoComplete;
            var textColor = (Color) newvalue;
            entryAutoComplete.SearchEntry.TextColor = textColor;
        }

        private static void OnPlaceholderColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var entryAutoComplete = bindable as EntryAutoComplete;
            var placeholderColor = (Color) newvalue;
            entryAutoComplete.SearchEntry.PlaceholderColor = placeholderColor;
        }

        private static void OnIsClearImageVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var entryAutoComplete = bindable as EntryAutoComplete;
            var isVisible = (bool) newValue;
            entryAutoComplete.ClearSearchEntryImage.IsVisible = isVisible;
        }

        private static void OnPlaceholderChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var entryAutoComplete = bindable as EntryAutoComplete;
            var placeholder = (string) newValue;
            entryAutoComplete.SearchEntry.Placeholder = placeholder;
        }

        private static void OnSearchTypeChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var entryAutoComplete = bindable as EntryAutoComplete;
            var searchType = (Mode) newvalue;
            entryAutoComplete.SearchType = searchType;
        }

        public string SearchText
        {
            get { return (string) GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public Color SearchTextColor
        {
            get { return (Color) GetValue(SearchTextColorProperty); }
            set { SetValue(SearchTextColorProperty, value); }
        }

        public int MaximumVisibleElements
        {
            get { return (int) GetValue(MaximumVisibleElementsProperty); }
            set { SetValue(MaximumVisibleElementsProperty, value); }
        }

        public int MinimumPrefixCharacter
        {
            get { return (int) GetValue(MinimumPrefixCharacterProperty); }
            set { SetValue(MinimumPrefixCharacterProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string Placeholder
        {
            get { return (string) GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public Color PlaceholderColor
        {
            get { return (Color) GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }

        public bool IsClearButtonVisible
        {
            get { return (bool) GetValue(IsClearButtonVisibleProperty); }
            set { SetValue(IsClearButtonVisibleProperty, value); }
        }

        public Mode SearchType
        {
            get { return (Mode) GetValue(SearchTypeProperty); }

            set { SetValue(SearchTypeProperty, value); }
        }

        private Grid Container;
        private Grid SearchEntryLayout;
        private ScrollView SuggestionWrapper;
        private StackLayout SuggestionsStackLayout;
        private Entry SearchEntry;
        private Image ClearSearchEntryImage;

        private IEnumerable _originSuggestions;

        public EntryAutoComplete()
        {
            InitGrid();
            InitSearchEntry();
            InitClearImage();
            InitSuggestionsListView();
            InitSuggestionsScrollView();

            PlaceControlsInGrid();

            Content = Container;
        }

        private void InitGrid()
        {
            Container = new Grid
            {
                RowSpacing = 0
            };
            Container.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            Container.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            Container.RowDefinitions.Add(new RowDefinition() { Height = 50 });
        }

        private void InitSearchEntry()
        {
            SearchEntry = new Entry
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
            SearchEntry.TextChanged += SearchEntry_TextChanged;
        }

        private void InitClearImage()
        {
            ClearSearchEntryImage = new Image
            {
                Source = "baseline_search_black_24.png",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                WidthRequest = 24,
                HeightRequest = 24,
                Margin = Device.OS!=TargetPlatform.Windows ? new Thickness(0,0,5,0) : new Thickness(0,0,10,15)
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (x, y) => SearchEntry.Text = "";
            ClearSearchEntryImage.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void InitSuggestionsListView()
        {
            SuggestionsStackLayout = new StackLayout
            {
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.End,
                Spacing = 0
            };
        }

        private void InitSuggestionsScrollView()
        {
            SuggestionWrapper = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
                BackgroundColor = Color.White,
                Content = SuggestionsStackLayout,
                IsVisible = false
            };
        }

        private void PlaceControlsInGrid()
        {
            SearchEntry.HorizontalOptions = new LayoutOptions
            {
                Alignment = LayoutAlignment.Fill,
                Expands = true
            };

            SearchEntryLayout = new Grid();
            SearchEntryLayout.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            SearchEntryLayout.RowDefinitions.Add(new RowDefinition() {Height = 50});

            SearchEntryLayout.Children.Add(SearchEntry, 0, 0);
            SearchEntryLayout.Children.Add(ClearSearchEntryImage, 0, 0);

            Container.Children.Add(SuggestionWrapper);
            Container.Children.Add(SearchEntryLayout, 0, 1);
        }

        private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var newSuggestions = _originSuggestions;
            SearchEntry_IconChanged(e);


            if (e.NewTextValue.Length >= MinimumPrefixCharacter)
            {
                newSuggestions = FilterSuggestions(newSuggestions, e.NewTextValue);
            }

            SuggestionWrapper.IsVisible = e.NewTextValue.Length != 0 &&
                                          e.NewTextValue.Length >= MinimumPrefixCharacter &&
                                          newSuggestions.Cast<object>().Count() != 0;
            SuggestionWrapper.IsEnabled = newSuggestions.Cast<object>().Count() >= MaximumVisibleElements;

            SuggestionsStackLayout.Children.Clear();
            foreach(var item in newSuggestions)
            {
                SuggestionsStackLayout.Children.Add(new Label
                {
                    Text = item.ToString(),
                    HeightRequest = RowHeight,
                    VerticalTextAlignment = TextAlignment.Center
                });
            }

            if (SuggestionWrapper.IsVisible)
            {
                UpdateLayout();
            }
        }

        private void SearchEntry_IconChanged(TextChangedEventArgs e)
        {
            ClearSearchEntryImage.Source = string.IsNullOrEmpty(e.NewTextValue)
                ? "baseline_search_black_24.png"
                : "baseline_close_black_24.png";
        }

        private IEnumerable FilterSuggestions(IEnumerable itemsSource, string searchText)
        {
            switch (SearchType)
            {
                case Mode.StartsWith:
                    return itemsSource.Cast<object>().Where(x => x.ToString().Equals(searchText) || 
                                                                 x.ToString().ToLower().StartsWith(searchText.ToLower()));
                case Mode.EndsWith:
                    return itemsSource.Cast<object>().Where(x => x.ToString().Equals(searchText) || 
                                                                 x.ToString().ToLower().EndsWith(searchText.ToLower()));
                case Mode.Contains:
                    return itemsSource.Cast<object>().Where(x => x.ToString().Equals(searchText) || 
                                                                 x.ToString().ToLower().Contains(searchText.ToLower()));
            }

            return itemsSource;
        }

        private int GetSuggestionsListHeight()
        {
            var items = SuggestionsStackLayout.Children.Cast<object>().ToList();
            return items.ToList().Count >= MaximumVisibleElements
                ? MaximumVisibleElements * RowHeight
                : items.Count * RowHeight;
        }

        private void UpdateLayout()
        {
            var listHeight = GetSuggestionsListHeight();
            SuggestionWrapper.HeightRequest = listHeight;
            Container.HeightRequest = listHeight + SearchEntryLayout.Height;
            Container.ForceLayout();
        }
    }
}