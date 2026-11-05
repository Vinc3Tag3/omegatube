using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmegaTube.Data;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using static OmegaTube.I18n;

namespace OmegaTube.Ui
{
    // Library hub: History, Watch later, Liked videos, Playlists (signed-in account).
    public class YouScreen : Screen
    {
        private class Entry
        {
            public string Title, Key;
            public Func<string, Task<FeedPage>> Loader;
            public bool Shelves;
            public string EmptyText;
        }

        private const int ListX = 120, ListY = 220, RowH = 88;
        private readonly List<Entry> entries = new List<Entry>();
        private readonly List<TextLabel> rows = new List<TextLabel>();
        private readonly View menuView;
        private readonly View feedHost;
        private readonly TextLabel statusLabel;
        private IFeedView feed;
        private int index;
        private bool inFeed;
        private string historySearch = "";
        private HistoryCommands historyCommands;

        public YouScreen(App app) : base(app)
        {
            Root.BackgroundColor = Theme.Background;
            var title = Widgets.Label(T("You"), Theme.TitleSize + 12, Theme.Text, 1200, 70);
            title.Position = new Position(ListX, 64);
            Root.Add(title);

            menuView = new View { Size = new Size(1920, 1080) };
            feedHost = new View { Position = new Position(Theme.RailWidth + 20, 110), Size = new Size(1920 - Theme.RailWidth - 40, 900) };
            feedHost.Hide();
            Root.Add(menuView);
            Root.Add(feedHost);

            statusLabel = Widgets.Label("", Theme.BodySize, Theme.TextDim, 1600, 40);
            statusLabel.Position = new Position(ListX, 140);
            menuView.Add(statusLabel);

            entries.Add(new Entry
            {
                Key = "history", Title = T("History"),
                Shelves = true,
                Loader = LoadHistory,
                EmptyText = T("Sign in to your account (Account menu) to see your history."),
            });
            entries.Add(new Entry
            {
                Key = "wl", Title = T("Watch later"),
                Loader = c => YouTube.PlaylistPage("WL", c),
                EmptyText = T("No videos in Watch later."),
            });
            entries.Add(new Entry
            {
                Key = "ll", Title = T("Liked videos"),
                Loader = c => YouTube.PlaylistPage("LL", c),
                EmptyText = T("No liked videos yet."),
            });
            entries.Add(new Entry
            {
                Key = "library", Title = T("Playlists"),
                Loader = c => YouTube.Browse("FElibrary", c),
                EmptyText = T("No playlists in your library."),
            });

            for (int i = 0; i < entries.Count; i++)
            {
                var row = Widgets.Label("  " + entries[i].Title + "  ", Theme.TitleSize - 4, Theme.Text, 1400, RowH);
                row.Position = new Position(ListX, ListY + i * (RowH + 12));
                row.VerticalAlignment = VerticalAlignment.Center;
                menuView.Add(row);
                rows.Add(row);
            }
            UpdateFocus();
        }

        private async Task<FeedPage> LoadHistory(string continuation)
        {
            FeedPage page = await YouTube.Browse("FEhistory", continuation);
            if (!string.IsNullOrEmpty(page.Error) || continuation.Length > 0)
            {
                return page;
            }
            historyCommands = page.Commands;
            if (page.Groups.Count > 0)
            {
                page.Shelves.Clear();
                foreach (HistoryGroup group in page.Groups)
                {
                    List<VideoItem> items = FilterHistory(group.Items);
                    if (items.Count > 0)
                    {
                        page.Shelves.Add(new Shelf { Title = group.Title, Items = items });
                    }
                }
                page.Items.Clear();
            }
            return page;
        }

        private List<VideoItem> FilterHistory(List<VideoItem> list)
        {
            if (string.IsNullOrWhiteSpace(historySearch))
            {
                return list;
            }
            string q = historySearch.Trim().ToLowerInvariant();
            var outList = new List<VideoItem>();
            foreach (VideoItem item in list)
            {
                if (item.Title != null && item.Title.ToLowerInvariant().Contains(q))
                {
                    outList.Add(item);
                }
            }
            return outList;
        }

        private void OpenEntry(int i)
        {
            if (!YouTube.SignedIn())
            {
                statusLabel.Text = T("Sign in from the Account screen, then try again.");
                return;
            }
            statusLabel.Text = "";
            Entry e = entries[i];
            inFeed = true;
            menuView.Hide();
            feedHost.Show();
            if (feed != null)
            {
                feedHost.Remove(feed as View);
                (feed as View)?.Dispose();
            }
            if (e.Shelves)
            {
                var shelves = new ShelfList(feedHost.SizeWidth, feedHost.SizeHeight);
                shelves.Show(e.Loader, "you:" + e.Key);
                feed = shelves;
            }
            else
            {
                var grid = new FeedGrid(feedHost.SizeWidth, feedHost.SizeHeight);
                grid.Show(e.Loader, "you:" + e.Key, e.EmptyText);
                feed = grid;
            }
            feedHost.Add(feed as View);
            feed.SetFocused(true);
        }

        private void CloseFeed()
        {
            inFeed = false;
            feed?.SetFocused(false);
            feedHost.Hide();
            menuView.Show();
            UpdateFocus();
        }

        private void UpdateFocus()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].BackgroundColor = !inFeed && i == index ? Theme.Accent : Color.Transparent;
            }
            feed?.SetFocused(inFeed);
        }

        public override bool OnKey(string key)
        {
            if (inFeed)
            {
                if (key == "XF86Back" || key == "Escape")
                {
                    CloseFeed();
                    UpdateFocus();
                    return true;
                }
                if (key == "Return" && feed != null && feed.OnReturnPress())
                {
                    return true;
                }
                if (key == "Return" && feed?.FocusedItem != null)
                {
                    App.Play(feed.FocusedItem);
                    return true;
                }
                if (feed != null && !feed.Move(key))
                {
                    return true;
                }
                return true;
            }
            switch (key)
            {
                case "Up":
                    index = Math.Max(0, index - 1);
                    break;
                case "Down":
                    index = Math.Min(entries.Count - 1, index + 1);
                    break;
                case "Return":
                    OpenEntry(index);
                    break;
                default:
                    if (key == "XF86Back" || key == "Escape")
                    {
                        return false;
                    }
                    return true;
            }
            UpdateFocus();
            return true;
        }
    }
}
