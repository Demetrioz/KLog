---
sidebar_position: 3
---

# Search for Logs

To search for specific logs, you can navigate to the "Investigate" section of
the navigation drawer.

From here, you can specify a date range, which applications you want to include
in the search, and if you would like to search the source, subject, message,
or level.

![Investigate](https://stkevinwilliamsdev.blob.core.windows.net/klog/investigate.PNG)

## Searching for log levels

You can search for log levels or other fields individually, or at the same time.

![Search](https://stkevinwilliamsdev.blob.core.windows.net/klog/search_bar.PNG)

### Log Levels only

To search for log levels only, specify "Info" "Warning" "Error" or "Critical" in
the search box, and make sure that the "Level" checkbox is checked in the filter
menu.

### Source, Subject, Message only

To search for the source, subject, or message only, type your phrase in the
search box, and make sure that the associated checkbox is checked in the filter
menu

### Log level and Source, Subject, or Message

If you would like to specify a log level, in addition to the text you're
looking for, make sure all appropriate checkboxes are checked in the filter menu
and format the search string as follows

```
<LOG_LEVEL>//<MESSAGE>

# example
Info//MySearchText
```
