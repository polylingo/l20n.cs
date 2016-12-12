docs:
	git checkout master

	mkdir -p bin/docs || true
	doxygen Doxyfile
	mv bin/docs/html newdocs
	git checkout gh-pages
	rm -rf docs && mv newdocs docs
