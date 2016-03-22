DZX-Forth versions
==================

The development of DZX-Forth started on 2014-11-03.

Legend: "\+" = new, "\-" = removed, "\*" = fixed, "\!" = changed

Version A-00-20150112
---------------------

First usable version. Keyboard input and screen output work.

* \* `emit`.
* \+ `border`.
* \+ `toupper`, `tolower`, `lower`.
* \* `bye`.
* \* `return`.
* \* `key`, `key?`.
* \* `type`.
* \+ `boot-message`.
* \* `quit`, `accept`, `interpret`.
* \+ `alias`.
* \+ "%" number prefix.
* \+ `binary`.
* \- `not`.
* \* `page`.
* \! `@execute`.
* \+ `@?execute`.

Version A-01-20150114
---------------------

Main improvement: Keyboard translation table: any typed
character can be converted to any code.  In the future, it will
be a conditional compilation option, with some additional
configuration options.

* \+ `get-user`, `set-user`, `restore-user`.
* \+ `tib`, `#tib`.
* \* `dos-version`.
* \+ `unused-disk`.
* \+ `get-drive`, `set-drive`.
* \+ Single-quoted chars as a number format.
* \+ Keyboard translation table.

Version A-02-20150214
---------------------

Main improvements: Most of the basic file words are implemented
(only `read-line` still waits); source block files work: the
system boots from disk, not from tape; circular string buffer.

* \+ `defer!`, `defer@`.
* \+ `alias`.
* \+ `hex.`.
* \+ `file-position`.
* \+ `paper`, `ink`, `bright`, `flash`, `normal`.
* \+ `create-file`, `close-file`.
* \+ `cell`, `4`, `float`.
* \+ `delete-file`.
* \! Now block files support is optional, by conditional compilation.
* \+ `write-file`.
* \+ `rdrop`.
* \+ `write-line`, `read-file`, `open-file`.
* \! `token` renamed to `parse-name`, after Gforth and Forth 2012.
* \+ `file-id`.
* \+ `reposition-file`, `abandon-file`, `headed`, `file-size`.
* \+ `read-byte`, `write-byte`.
* \- `filetype?`.
* \+ `filetype`.
* \! `zbuf` renamed to `filename-buffers`.
* \! `lastfile` renamed to `last-filename`.
* \- `.lastfile`.
* \! `>fname`renamed to `>filename-buffer`.
* \+ `rename-file`.
* \! Faster `+string`.
* \+ `control>bl`.
* \+ Block files can be opened and loaded.
* \+ Circular string buffer.
* \! No TAP file required anymore: now the system boots from disk.

Version A-03-20150219
---------------------

Main improvement: `read-line`.

* \! `(name)` and `n>count` converted to `name>string` (after Gforth and Forth-2012).
* \- `>ior`.
* \! `(read-file)` renamed to `page-read-file`.
* \! `(write-file)` renamed to `page-write-file`.
* \* +3DOS errors are changed to negative (range -1000..-1036, with additions from -1100).
* \+ `read-line`.

Version A-03-xxxxxxxx (under development)
-----------------------------------------

* \+ `char-`
* \! `>filename-buffer` is substituted by `>filename`, that
  returns the string ready to be used by +3DOS and uses the
  circular string buffer when available.
* \+ `?exit`, after Gforth.
* \* Faster and simpler `control>bl`: all control characters
  are converted to spaces.
* \+ `eof?`, needed to make `file-read` standard.
* \+ `flush-drive`
* \+ `basic-file-header`
* \+ `idedos?`
