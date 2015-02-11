node-dump
===

Make a dump of the V8 heap and cpu for later inspection.

### Install

    npm install ndump

### Build

    node-gyp configure build

### Usage

Load the add-on in your application:

    var ndump = require('ndump');

The module exports both heap and cpu method.


the first called `heap(filepath)` that
writes a  file to the args directory.

    ndump.heap(filepath);
 
 the second  called  `cpu(filepath,times)` that
writes a  file to the args directory.

    ndump.cpu(filepath,times);

### Inspecting the snapshot

run this function on http web server,download or save on local disk then.

Open [Google Chrome](https://www.google.com/intl/en/chrome/browser/) and
press F12 to open the developer toolbar.

Go to the `Profiles` tab, right-click in the tab pane and select
`Load profile...`.

Select the dump file and click `Open`.  You can now inspect the heap snapshot
at your leisure.
