# This isn't for compiling; Unity does that.
# Rather, this is for building the documentation.
# If any scripts which are being watched by Doxygen are updated,
# please run this!
#
# If you have a Linux system (or WSL on Windows... or Mac),
# and with both doxygen and make installed, you can just run
#
#   make
#
# and that'll generate the documentation.

root=..
doxyPath=.

.PHONY: docs
docs:
	rm -rf $(root)/docs
	doxygen $(doxyPath)/Doxyfile
	mv $(root)/docs/html/* $(root)/docs/
	rmdir $(root)/docs/html
