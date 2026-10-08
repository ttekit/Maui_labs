using OOP.Lab6.Core;

await Object2SyncService.SyncAsync().ConfigureAwait(false);

using var watcher = new Lab6DataWatcher(() => _ = Object2SyncService.SyncAsync());
await Task.Delay(Timeout.InfiniteTimeSpan).ConfigureAwait(false);
