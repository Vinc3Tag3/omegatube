using System.Collections.Generic;
using System.Threading.Tasks;
using OmegaTube.Config;
using OmegaTube.Data;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using static OmegaTube.I18n;

namespace OmegaTube.Ui
{
    // Keyboard (app layout) + recent searches + suggestions; after search, full-height FeedGrid + filters.
    public class SearchScreen : Screen
    {
        private static readonly string[][] Qwerty =
        {
            new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" },
            new[] { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p" },
            new[] { "a", "s", "d", "f", "g", "h", "j", "k", "l", "'" },
            new[] { "z", "x", "c", "v", "b", "n", "m", ",", ".", "-" },
        };
        private static readonly string[][] Azerty =
        {
            new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" },
            new[] { "a", "z", "e", "r", "t", "y", "u", "i", "o", "p" },
            new[] { "q", "s", "d", "f", "g", "h", "j", "k", "l", "m" },
            new[] { "w", "x", "c", "v", "b", "n", "é", "'", ".", "-" },
        };
        private static readonly string[][] QwertySpanish =
        {
            new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" },
            new[] { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p" },
            new[] { "a", "s", "d", "f", "g", "h", "j", "k", "l", "ñ" },
            new[] { "z", "x", "c", "v", "b", "n", "m", ",", ".", "-" },
        };
        private static readonly string[][] Symbols =
        {
            new[] { "à", "â", "ä", "á", "é", "è", "ê", "ë", "í", "î" },
            new[] { "ï", "ó", "ô", "ö", "ú", "ù", "û", "ü", "ç", "ñ" },
            new[] { "œ", "æ", "ß", "¿", "¡", "?", "!", "&", "#", "@" },
            new[] { ":", ";", "/", "+", "=", "(", ")", "\"", "'", "_" },
        };
        private enum KeyAction { Page, Space, Delete, Clear, Search }
        private const int CharRows = 4, ActionRow = 4, ActionKeys = 5;
        private const int KeyW = 86, KeyH = 72, KeyGap = 8;
        private const int KbX = 80, KbY = 190;
        private const int RightX = 1060;
        private const int ResultTop = 200;
        private const int FilterCount = 8;

        private enum Zone { Keyboard, Recent, Suggestions, AddButton, Filters, Results, EditQuery }

        private readonly TextLabel queryLabel;
        private readonly TextLabel addButton;
        private readonly View composePanel;
        private readonly View resultPanel;
        private readonly TextLabel resultQueryLabel;
        private readonly TextLabel editQueryBtn;
        private readonly View filterHost;
        private readonly List<TextLabel> filterPills = new List<TextLabel>();
        private readonly FeedGrid resultGrid;
        private readonly List<List<TextLabel>> keyLabels = new List<List<TextLabel>>();
        private readonly TextLabel recentTitle;
        private readonly List<TextLabel> recentLabels = new List<TextLabel>();
        private readonly TextLabel sugTitle;
        private readonly List<TextLabel> suggestionLabels = new List<TextLabel>();

        private List<string> suggestions = new List<string>();
        private string query = "";
        private string lastQuery = "";
        private readonly SearchFilters filters = new SearchFilters();
        private bool resultMode;
        private Zone zone = Zone.Keyboard;
        private int keyRow = 1, keyCol;
        private int recentIndex;
        private int suggestionIndex;
        private int filterIndex;
        private int suggestGeneration;
        private bool symbolPage;
        private string[][] Layout => symbolPage ? Symbols
                                   : Lang == "fr" ? Azerty : Lang == "es" ? QwertySpanish : Qwerty;

        public SearchScreen(App app) : base(app)
        {
            Root.BackgroundColor = Theme.Background;

            composePanel = new View { Size = new Size(1920, 1080) };
            resultPanel = new View { Size = new Size(1920, 1080) };
            resultPanel.Hide();
            Root.Add(composePanel);
            Root.Add(resultPanel);

            var box = Widgets.Box(Theme.Surface, 900, 80);
            box.Position = new Position(KbX, 70);
            composePanel.Add(box);
            queryLabel = Widgets.Label("", Theme.TitleSize, Theme.Text, 860, 80);
            queryLabel.Position = new Position(20, 0);
            queryLabel.VerticalAlignment = VerticalAlignment.Center;
            box.Add(queryLabel);

            addButton = Widgets.Label("  + " + T("Add to menu") + "  ", Theme.BodySize, Theme.Text, 0, 0);
            addButton.Position = new Position(KbX + 930, 86);
            addButton.BackgroundColor = Theme.Surface;
            addButton.Hide();
            composePanel.Add(addButton);

            for (int r = 0; r <= ActionRow; r++)
            {
                var line = new List<TextLabel>();
                int x = KbX;
                int count = r == ActionRow ? ActionKeys : 10;
                for (int c = 0; c < count; c++)
                {
                    int w = r == ActionRow ? (10 * (KeyW + KeyGap) - KeyGap - (ActionKeys - 1) * KeyGap) / ActionKeys : KeyW;
                    var key = Widgets.Label("", r == ActionRow ? Theme.BodySize + 1 : Theme.BodySize + 4, Theme.Text, w, KeyH);
                    key.HorizontalAlignment = HorizontalAlignment.Center;
                    key.VerticalAlignment = VerticalAlignment.Center;
                    key.BackgroundColor = Theme.Surface;
                    key.Position = new Position(x, KbY + r * (KeyH + KeyGap));
                    composePanel.Add(key);
                    line.Add(key);
                    x += w + KeyGap;
                }
                keyLabels.Add(line);
            }
            LabelKeys();

            recentTitle = Widgets.Label(T("Recent searches"), Theme.BodySize, Theme.TextDim, 780);
            recentTitle.Position = new Position(RightX, 190);
            composePanel.Add(recentTitle);
            for (int i = 0; i < 7; i++)
            {
                var r = Widgets.Label("", Theme.BodySize + 2, Theme.Text, 780, 50);
                r.Position = new Position(RightX, 240 + i * 54);
                r.VerticalAlignment = VerticalAlignment.Center;
                composePanel.Add(r);
                recentLabels.Add(r);
            }

            sugTitle = Widgets.Label(T("Suggestions"), Theme.BodySize, Theme.TextDim, 780);
            sugTitle.Position = new Position(RightX, 580);
            composePanel.Add(sugTitle);
            for (int i = 0; i < 7; i++)
            {
                var s = Widgets.Label("", Theme.BodySize + 2, Theme.Text, 780, 50);
                s.Position = new Position(RightX, 630 + i * 54);
                s.VerticalAlignment = VerticalAlignment.Center;
                composePanel.Add(s);
                suggestionLabels.Add(s);
            }

            resultQueryLabel = Widgets.Label("", Theme.TitleSize, Theme.Text, 1200, 80);
            resultQueryLabel.Position = new Position(KbX, 70);
            resultQueryLabel.VerticalAlignment = VerticalAlignment.Center;
            resultPanel.Add(resultQueryLabel);

            editQueryBtn = Widgets.Label("  " + T("Edit search") + "  ", Theme.BodySize, Theme.Text, 0, 0);
            editQueryBtn.Position = new Position(KbX + 1220, 86);
            editQueryBtn.BackgroundColor = Theme.Surface;
            resultPanel.Add(editQueryBtn);

            filterHost = new View { Position = new Position(KbX, 150), Size = new Size(1760, 44) };
            resultPanel.Add(filterHost);
            for (int i = 0; i < FilterCount; i++)
            {
                var pill = Widgets.Label("", Theme.BodySize - 2, Theme.Text, 0, 40);
                pill.BackgroundColor = Theme.Surface;
                filterHost.Add(pill);
                filterPills.Add(pill);
            }

            resultGrid = new FeedGrid(1760, 1080 - ResultTop - 40);
            resultGrid.Position = new Position(KbX - Theme.FocusMargin, ResultTop);
            resultPanel.Add(resultGrid);

            UpdateFocus();
        }

        public override void OnShow()
        {
            RefreshRecent();
            UpdateFocus();
        }

        private void RefreshRecent()
        {
            IReadOnlyList<string> list = LocalStore.RecentSearches;
            recentTitle.Show();
            for (int i = 0; i < recentLabels.Count; i++)
            {
                if (i < list.Count)
                {
                    recentLabels[i].Text = "  " + list[i];
                    recentLabels[i].Show();
                }
                else
                {
                    recentLabels[i].Text = "";
                    recentLabels[i].Hide();
                }
            }
            if (list.Count == 0)
            {
                recentTitle.Hide();
            }
        }

        private int RecentCount => System.Math.Min(LocalStore.RecentSearches.Count, recentLabels.Count);
        private int SuggestionCount => System.Math.Min(suggestions.Count, suggestionLabels.Count);

        private void LabelKeys()
        {
            string[][] layout = Layout;
            for (int r = 0; r < CharRows; r++)
            {
                for (int c = 0; c < 10; c++)
                {
                    keyLabels[r][c].Text = layout[r][c];
                }
            }
            List<TextLabel> actions = keyLabels[ActionRow];
            actions[(int)KeyAction.Page].Text = symbolPage ? "abc" : "àç?#";
            actions[(int)KeyAction.Space].Text = T("Space");
            actions[(int)KeyAction.Delete].Text = "← " + T("Delete");
            actions[(int)KeyAction.Clear].Text = T("Clear");
            actions[(int)KeyAction.Search].Text = T("Search");
        }

        private void LayoutFilterPills()
        {
            const int pillW = 220;
            for (int i = 0; i < filterPills.Count; i++)
            {
                var pill = filterPills[i];
                pill.Text = "  " + FilterCaption(i) + "  ";
                pill.Size = new Size(pillW, 40);
                pill.Position = new Position(i * (pillW + 12), 0);
            }
        }

        private string FilterCaption(int i)
        {
            switch (i)
            {
                case 0:
                    return filters.IsDefault ? T("All filters") : T("Reset filters");
                case 1:
                    return T("Duration: {0}", T("filter_duration_" + filters.Duration));
                case 2:
                    return T("Upload date: {0}", T("filter_date_" + filters.Date));
                case 3:
                    return T("Type: {0}", T("filter_type_" + filters.Type));
                case 4:
                    return T("Sort: {0}", T("filter_sort_" + filters.Sort));
                case 5:
                    return filters.Live ? T("Live on") : T("Live off");
                case 6:
                    return filters.Hd ? T("HD on") : T("HD off");
                default:
                    return filters.Subtitles ? T("Subtitles on") : T("Subtitles off");
            }
        }

        private void ApplyFilterAction(int i)
        {
            if (i == 0)
            {
                filters.Date = filters.Duration = filters.Type = "any";
                filters.Sort = "relevance";
                filters.Live = filters.Hd = filters.Subtitles = false;
            }
            else if (i == 1)
            {
                filters.Duration = Cycle(filters.Duration, new[] { "any", "short", "medium", "long" });
            }
            else if (i == 2)
            {
                filters.Date = Cycle(filters.Date, new[] { "any", "hour", "today", "week", "month", "year" });
            }
            else if (i == 3)
            {
                filters.Type = Cycle(filters.Type, new[] { "any", "video", "channel", "playlist" });
            }
            else if (i == 4)
            {
                filters.Sort = Cycle(filters.Sort, new[] { "relevance", "date", "views" });
            }
            else if (i == 5)
            {
                filters.Live = !filters.Live;
            }
            else if (i == 6)
            {
                filters.Hd = !filters.Hd;
            }
            else
            {
                filters.Subtitles = !filters.Subtitles;
            }
            LayoutFilterPills();
            ReloadResults();
        }

        private static string Cycle(string current, string[] opts)
        {
            for (int i = 0; i < opts.Length; i++)
            {
                if (opts[i] == current)
                {
                    return opts[(i + 1) % opts.Length];
                }
            }
            return opts[0];
        }

        private void SetResultMode(bool on)
        {
            resultMode = on;
            if (on)
            {
                composePanel.Hide();
                resultPanel.Show();
            }
            else
            {
                resultPanel.Hide();
                composePanel.Show();
            }
            if (on)
            {
                resultQueryLabel.Text = T("Search: {0}", lastQuery);
                LayoutFilterPills();
                zone = Zone.Results;
                resultGrid.SetFocused(true);
            }
            else
            {
                resultGrid.SetFocused(false);
                zone = Zone.Keyboard;
                RefreshRecent();
            }
            UpdateFocus();
        }

        private void ReloadResults()
        {
            string cache = "search:" + lastQuery + ":" + (filters.ToJson() ?? "default");
            resultGrid.Show(c => YouTube.Search(lastQuery, c, filters.ToJson()), cache, T("No results for this search"));
        }

        private void UpdateFocus()
        {
            if (!resultMode)
            {
                queryLabel.Text = query.Length > 0 ? query + "_" : T("Search YouTube…");
                queryLabel.TextColor = query.Length > 0 ? Theme.Text : Theme.TextDim;
                for (int r = 0; r < keyLabels.Count; r++)
                {
                    for (int c = 0; c < keyLabels[r].Count; c++)
                    {
                        bool f = zone == Zone.Keyboard && r == keyRow && c == keyCol;
                        keyLabels[r][c].BackgroundColor = f ? Theme.Accent : Theme.Surface;
                    }
                }
                for (int i = 0; i < recentLabels.Count; i++)
                {
                    bool vis = i < LocalStore.RecentSearches.Count;
                    if (vis)
                    {
                        recentLabels[i].BackgroundColor = zone == Zone.Recent && i == recentIndex
                            ? Theme.SurfaceFocused : Color.Transparent;
                    }
                }
                for (int i = 0; i < suggestionLabels.Count; i++)
                {
                    suggestionLabels[i].Text = i < suggestions.Count ? "  " + suggestions[i] : "";
                    suggestionLabels[i].BackgroundColor = zone == Zone.Suggestions && i == suggestionIndex
                        ? Theme.SurfaceFocused : Color.Transparent;
                }
                addButton.BackgroundColor = zone == Zone.AddButton ? Theme.Accent : Theme.Surface;
            }
            else
            {
                editQueryBtn.BackgroundColor = zone == Zone.EditQuery ? Theme.Accent : Theme.Surface;
                for (int i = 0; i < filterPills.Count; i++)
                {
                    filterPills[i].BackgroundColor = zone == Zone.Filters && i == filterIndex ? Theme.Accent : Theme.Surface;
                }
                resultGrid.SetFocused(zone == Zone.Results);
            }
        }

        public override bool OnKey(string key)
        {
            if (resultMode)
            {
                return ResultKey(key);
            }
            switch (zone)
            {
                case Zone.Keyboard:
                    if (!KeyboardKey(key))
                    {
                        return false;
                    }
                    break;
                case Zone.Recent:
                    if (key == "Up")
                    {
                        recentIndex = System.Math.Max(0, recentIndex - 1);
                    }
                    else if (key == "Down")
                    {
                        recentIndex = System.Math.Min(RecentCount - 1, recentIndex + 1);
                    }
                    else if (key == "Left")
                    {
                        zone = Zone.Keyboard;
                    }
                    else if (key == "Right" && SuggestionCount > 0)
                    {
                        zone = Zone.Suggestions;
                        suggestionIndex = 0;
                    }
                    else if (key == "Return" && recentIndex < RecentCount)
                    {
                        query = LocalStore.RecentSearches[recentIndex];
                        RunSearch();
                    }
                    else if (key == "Delete" && recentIndex < RecentCount)
                    {
                        LocalStore.RemoveRecentSearch(LocalStore.RecentSearches[recentIndex]);
                        recentIndex = System.Math.Min(recentIndex, RecentCount - 1);
                        RefreshRecent();
                    }
                    else if (key == "Clear")
                    {
                        LocalStore.ClearRecentSearches();
                        recentIndex = 0;
                        RefreshRecent();
                        zone = Zone.Keyboard;
                    }
                    else if (key == "XF86Back" || key == "Escape")
                    {
                        zone = Zone.Keyboard;
                    }
                    break;
                case Zone.Suggestions:
                    if (key == "Up")
                    {
                        if (suggestionIndex == 0 && RecentCount > 0)
                        {
                            zone = Zone.Recent;
                            recentIndex = System.Math.Min(RecentCount - 1, suggestionIndex);
                        }
                        else
                        {
                            suggestionIndex = System.Math.Max(0, suggestionIndex - 1);
                        }
                    }
                    else if (key == "Down")
                    {
                        suggestionIndex = System.Math.Min(SuggestionCount - 1, suggestionIndex + 1);
                    }
                    else if (key == "Left")
                    {
                        zone = RecentCount > 0 ? Zone.Recent : Zone.Keyboard;
                    }
                    else if (key == "Return" && suggestionIndex < suggestions.Count)
                    {
                        query = suggestions[suggestionIndex];
                        RunSearch();
                    }
                    else if (key == "XF86Back" || key == "Escape")
                    {
                        zone = Zone.Keyboard;
                    }
                    break;
                case Zone.AddButton:
                    if (key == "Down" || key == "XF86Back" || key == "Escape")
                    {
                        zone = Zone.Keyboard;
                    }
                    else if (key == "Return")
                    {
                        AddToHome();
                    }
                    break;
            }
            UpdateFocus();
            return true;
        }

        private bool ResultKey(string key)
        {
            switch (zone)
            {
                case Zone.Results:
                    if (key == "Up")
                    {
                        if (!resultGrid.Move("Up"))
                        {
                            zone = Zone.Filters;
                            filterIndex = 0;
                        }
                    }
                    else if (key == "Left" && !resultGrid.Move("Left"))
                    {
                        return false;
                    }
                    else if (key == "Right")
                    {
                        resultGrid.Move("Right");
                    }
                    else if (key == "Down")
                    {
                        resultGrid.Move("Down");
                    }
                    else if (key == "Return" && resultGrid.FocusedItem != null)
                    {
                        App.Play(resultGrid.FocusedItem);
                    }
                    else if (key == "XF86Back" || key == "Escape")
                    {
                        SetResultMode(false);
                    }
                    break;
                case Zone.Filters:
                    if (key == "Left")
                    {
                        filterIndex = System.Math.Max(0, filterIndex - 1);
                    }
                    else if (key == "Right")
                    {
                        filterIndex = System.Math.Min(FilterCount - 1, filterIndex + 1);
                    }
                    else if (key == "Down" || key == "Return")
                    {
                        if (key == "Return")
                        {
                            ApplyFilterAction(filterIndex);
                        }
                        zone = Zone.Results;
                    }
                    else if (key == "Up")
                    {
                        zone = Zone.EditQuery;
                    }
                    else if (key == "XF86Back" || key == "Escape")
                    {
                        SetResultMode(false);
                    }
                    break;
                case Zone.EditQuery:
                    if (key == "Down" || key == "Return")
                    {
                        if (key == "Return")
                        {
                            SetResultMode(false);
                        }
                        else
                        {
                            zone = Zone.Filters;
                        }
                    }
                    else if (key == "XF86Back" || key == "Escape")
                    {
                        SetResultMode(false);
                    }
                    break;
            }
            UpdateFocus();
            return true;
        }

        private bool KeyboardKey(string key)
        {
            switch (key)
            {
                case "Up":
                    if (keyRow == 0)
                    {
                        if (lastQuery.Length > 0)
                        {
                            zone = Zone.AddButton;
                        }
                    }
                    else
                    {
                        keyCol = keyRow == ActionRow ? System.Math.Min(9, keyCol * 10 / ActionKeys) : keyCol;
                        keyRow--;
                    }
                    break;
                case "Down":
                    if (keyRow == ActionRow)
                    {
                        if (RecentCount > 0)
                        {
                            zone = Zone.Recent;
                            recentIndex = 0;
                        }
                        else if (SuggestionCount > 0)
                        {
                            zone = Zone.Suggestions;
                            suggestionIndex = 0;
                        }
                    }
                    else
                    {
                        keyRow++;
                        keyCol = keyRow == ActionRow ? System.Math.Min(ActionKeys - 1, keyCol * ActionKeys / 10) : keyCol;
                    }
                    break;
                case "Left":
                    if (keyCol == 0)
                    {
                        return false;
                    }
                    keyCol--;
                    break;
                case "Right":
                    if (keyCol == keyLabels[keyRow].Count - 1)
                    {
                        if (RecentCount > 0)
                        {
                            zone = Zone.Recent;
                            recentIndex = 0;
                        }
                        else if (SuggestionCount > 0)
                        {
                            zone = Zone.Suggestions;
                            suggestionIndex = 0;
                        }
                    }
                    else
                    {
                        keyCol++;
                    }
                    break;
                case "Return":
                    if (keyRow == ActionRow)
                    {
                        PressAction((KeyAction)keyCol);
                    }
                    else
                    {
                        query += Layout[keyRow][keyCol];
                        UiGuard.Fire("SearchScreen.RefreshSuggestions", RefreshSuggestions);
                    }
                    break;
                case "XF86Back":
                case "Escape":
                    return false;
            }
            return true;
        }

        private void PressAction(KeyAction action)
        {
            switch (action)
            {
                case KeyAction.Page:
                    symbolPage = !symbolPage;
                    LabelKeys();
                    return;
                case KeyAction.Space:
                    query += " ";
                    break;
                case KeyAction.Delete:
                    if (query.Length > 0)
                    {
                        query = query.Substring(0, query.Length - 1);
                    }
                    break;
                case KeyAction.Clear:
                    query = "";
                    break;
                case KeyAction.Search:
                    RunSearch();
                    return;
            }
            UiGuard.Fire("SearchScreen.RefreshSuggestions", RefreshSuggestions);
        }

        private async Task RefreshSuggestions()
        {
            int generation = ++suggestGeneration;
            await Task.Delay(250);
            if (generation != suggestGeneration)
            {
                return;
            }
            List<string> list = query.Trim().Length == 0 ? new List<string>() : await YouTubeSuggest(query);
            if (generation != suggestGeneration)
            {
                return;
            }
            suggestions = list;
            if (zone == Zone.Suggestions && SuggestionCount == 0)
            {
                zone = Zone.Keyboard;
            }
            UpdateFocus();
        }

        private static Task<List<string>> YouTubeSuggest(string q) => Data.YouTube.Suggest(q);

        private void RunSearch()
        {
            string q = query.Trim();
            if (q.Length == 0)
            {
                return;
            }
            lastQuery = q;
            LocalStore.AddRecentSearch(q);
            addButton.Show();
            ReloadResults();
            SetResultMode(true);
        }

        private void AddToHome()
        {
            if (string.IsNullOrEmpty(lastQuery))
            {
                return;
            }
            App.Config.HomeRows.Add(new HomeRow { Title = lastQuery, Query = lastQuery, Icon = HomeRow.GuessIcon(lastQuery) });
            App.Config.Save();
            App.Home.InvalidateRows();
            addButton.Text = "  ✓ " + T("Added to menu") + "  ";
        }
    }
}
