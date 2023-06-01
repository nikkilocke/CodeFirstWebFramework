var unsavedInput;	// True if an edit field has been changed, and not saved
// var testHarness;	// True if running in Firefox (used for automated tests)
var touchScreen;	// True if running on a tablet or phone
var doubleClickToSelect;	// Set to true if you want the user to have to tap twice to select items in data tables and list forms
var decPoint;		// The decimal point character of this locale
// Extend auto-complete widget to cope with multiple categories
$.widget("custom.catcomplete", $.ui.autocomplete, {
	_create: function () {
		this._super();
		this.widget().menu("option", "items", "> :not(.ui-autocomplete-category)");
	},
	_renderMenu: function (ul, items) {
		var that = this,
			currentCategory = "";
		$.each(items, function (index, item) {
			var li;
			if (item.category != currentCategory) {
				ul.append("<li class='ui-autocomplete-category'>" + item.category + "</li>");
				currentCategory = item.category;
			}
			li = that._renderItemData(ul, item);
			if (item.category) {
				li.attr("aria-label", item.category + " : " + item.label);
			}
		});
	}
});
var dateStyle = { dateFormat: 'dd MM yy' };
var True = true;
var False = false;

function addJQueryUiControls() {
}

function SafeHtml(htmlItem, col, rowno, suffix, attributes) {
	this.item = $('<div></div>');
	if (htmlItem)
		this.addTo(this.item, htmlItem, col, rowno, suffix, attributes);
}

/**
 * Add an item to the div, and return it
 * @param {string} htmlItem item name (e.g. 'input')
 * @param {any} col Optional column def (sets data-col to name)
 * @param {int} rowno Optional row number (sets id as well)
 * @param {string} suffix Optional suffix (added to id)
 * @param {any} attributes Optional attributes to set on the item
*/
SafeHtml.prototype.add = function (htmlItem, col, rowno, suffix, attributes) {
	return this.addTo(this.item, htmlItem, col, rowno, suffix, attributes);
}

/**
 * Add an item to the provided item, and return it
 * @param {jquery object} item the item to add to
 * @param {string} htmlItem item name (e.g. 'input')
 * @param {any} col Optional column def (sets data-col to name)
 * @param {int} rowno Optional row number (sets id as well)
 * @param {string} suffix Optional suffix (added to id)
 * @param {any} attributes Optional attributes to set on the item
*/
SafeHtml.prototype.addTo = function (item, htmlItem, col, rowno, suffix, attributes) {
	if (attributes === undefined) {
		var a = suffix;
		if (a === undefined)
			a = rowno;
		if (a === undefined)
			a = col;
		if (a && typeof (a) == 'object' && a.draw === undefined) {
			attributes = a;
			if (a === suffix)
				suffix = undefined;
			else if (a === rowno)
				rowno = undefined;
			else if (a === col)
				col = undefined;
		} else
			attributes = {};
	}
	if (col) {
		if (col.attributes) {
			var re = /([^ "=]+) *= *"([^"]*)"/g;
			for (; ;) {
				var m = re.exec(col.attributes);
				if (m)
					attributes[m[1]] = m[2];
				else
					break;
			}
		}

		if (rowno !== undefined)
			attributes.id = 'r' + rowno + 'c' + col.name + (suffix || '');
		attributes['data-col'] = col.name;
	}
	var result = $('<' + htmlItem + '>');
	for (a in attributes) {
		if (SafeHtml.properties[a]) {
			if (attributes[a])
				result.attr(a, a);
		} else if (a == 'text')
			result.text(attributes[a]);
		else
			result.attr(a, attributes[a]);
	}
	item.append(result);
	return result;
}

/**
 * Set the text of the div itself
 * @param {string} s the text
 */
SafeHtml.prototype.text = function (s) {
	this.item.text(s === null || s === undefined ? '' : s);
	return this;
}

/**
 * Return the html of the div
 */
SafeHtml.prototype.html = function () {
	return this.item.html();
}

/**
 * Which items are properties rather than attributes
 */
SafeHtml.properties = { checked: true, selected: true, disabled: true, multiple: true };

$(function () {
	//	testHarness = bowser.firefox && hasParameter('test');
	touchScreen = bowser.mobile || bowser.tablet;
	decPoint = (1.23).toLocaleString(window.navigator.userLanguage || window.navigator.language)[1];
	resize();
	$('#menuicon').click(function () {
		// Small screen user has clicked menu icon - show/hide menu
		$('#header').slideToggle(resize);
	});
	$('body').on('click', 'button[href]', function (e) {
		// Buttons with hrefs act like links
		var href = $(this).attr('href');
		var target = e.ctrlKey ? "_blank" : $(this).attr('target');
		if (target)
			window.open(href, target);
		else
			window.location = href;
	});
	$('body').on('click', 'button[data-goto]', function (e) {
		// Buttons with data-goto act like links, but also store state to come back to
		goto($(this).attr('data-goto'), e);
	});
	$(window).bind('beforeunload', function () {
		// Warn user if there is unsaved data on leaving the page
		if (unsavedInput) return "There is unsaved data";
	});
	$(window).on('resize', resize);
	$(window).on('unload', function () {
		// Disable input fields & buttons on page unload
		message("Please wait...");
		$(':input').prop('disabled', true);
	});
	$('body').on('change', ':input:not(.nosave)', function () {
		// User has changed an input field - set unsavedInput (except for dataTable search field)
		if (!$(this).attr('aria-controls'))
			unsavedInput = true;
	});
	$('button[type="submit"]').click(function () {
		// Submit button will presumably save the input
		message("Please wait...");
		unsavedInput = false;
	});
	$('body').on('click', 'button.reset', function () {
		// For when an ordinary reset button won't do any calculated data
		window.location.reload();
	});
	$('body').on('click', 'button.cancel', goback);
	$('body').on('click', 'button.nextButton', function () {
		// Button to set document number field to <next>, so C# will fill it in with the next one.
		$(this).prev('input[type="text"]').val('<next>').trigger('change');
	});
	$('body').on('click', 'table.form span.hint', function (e) {
		e.preventDefault();
		var showing = $(this).parent().find('#tooltip');
		$('#tooltip').remove();
		if (showing && showing.length)
			return;
		$(this).after('<span id="tooltip"></span>');
		var $title = $(this).parent().attr('title');
		$(this).next().append($title);
	});
	$('body').on('click', 'table.form #tooltip', function (e) {
		$('#tooltip').remove();
		e.preventDefault();
	});

	if (!touchScreen) {
		// Moving focus to a field selects the contents (except on touch screens)
		var focusInput;
		$('body').on('focus', ':input', function () {
			focusInput = this;
			$(this).select();
		}).on('mouseup', ':input', function (e) {
			if (focusInput == this) {
				focusInput = null;
				e.preventDefault();
			}
		});
	}
	var components = window.location.pathname.split('/');
	// Highlight top level menu item corresponding to current module
	$('#menu1 button[href="/' + components[1] + '/default.html"]').addClass('highlight');
	// Highlight second level menu item corresponding to current url
	$('#menu2 button').each(function () {
		var href = $(this).attr('href');
		if (href == window.location.pathname + window.location.search)
			$(this).addClass('highlight');
	});

	$("html").on("dragover", function (e) {
		e.preventDefault();
		e.stopPropagation();
	});

	// inlineImageInput processing
	$("html").on("drop", function (e) {
		e.preventDefault();
		e.stopPropagation();
	});

	$('body').on('paste', 'div.inlineImageInput', function (e) {
		// use event.originalEvent.clipboard for newer chrome versions
		updateImageInput.call(this, (e.clipboardData || e.originalEvent.clipboardData).items);
	});
	$('body').on('drop', 'div.inlineImageInput', function (e) {
		updateImageInput.call(this, e.originalEvent.dataTransfer.files);
	});

	$('body').on('change', 'div.inlineImageInput input[type=file]', function (e) {
		updateImageInput.call($(this).closest('div.inlineImageInput'), e.originalEvent.target.files);
	});
	$('body').on('click', 'img.autoDropdown', function (e) {
		var p = $(this).prev();
		p.focus();
		p.autocomplete("option", "minLength", 0);
		p.autocomplete("search", "");
		p.autocomplete("option", "minLength", 2);
	});

	setTimeout(function () {
		// Once initial form creation is done:
		//  add a Back button if there isn't one
		if (/[^\/]\//.test(window.location.pathname) && $('button#Back').length == 0)
			addBackButton();
		setFocusToFirstInputField();
	}, 100);
});

function updateImageInput(items) {
	// load image if there is oneimage
	for (var i = 0; i < items.length; i++) {
		if (items[i].type.indexOf("image") === 0) {
			var img = $(this).find('img');
			var inp = $(this).find('input.imageDummy');
			var reader = new FileReader();
			reader.onload = function (event) {
				img.prop('src', event.target.result);
				inp.val(event.target.result);
				inp.trigger('change');
			};
			reader.readAsDataURL(items[i]);
			break;
		}
	}
}

function addBackButton() {
	insertActionButton('Back', 'back').click(goback);
}

function setFocusToFirstInputField() {
	//  Focus to the first input field
	var focusField = $(':input[autofocus]:enabled:visible:first');
	if (focusField.length == 0)
		focusField = $(':input:enabled:visible:not(button):first');
	focusField.focus().select();
}

$(window).on("load", resize);

/**
 * Add a goto button to menu 2
 * @param text Button text
 * @param url Url to go to
 * @returns {*|jQuery} Button
 */
function addButton(text, url) {
	var btn = $('<button></button>')
		.attr('id', text.replace(/ /g, ''))
		.text(text)
		.appendTo($('#menu2').show());
	if (url)
		btn.attr('data-goto', url);
	resize();
	return btn;
}

/**
 * Add a link button to menu 2
 * @param text Button text
 * @param url Url to link to
 * @returns {*|jQuery} Button
 */
function jumpButton(text, url) {
	var btn = $('<button></button>')
		.attr('id', text.replace(/ /g, ''))
		.attr('href', url + window.location.search)
		.text(text)
		.appendTo($('#menu2').show());
	resize();
	return btn;
}

/**
 * Add a button to menu 3
 * @param text Button text
 * @returns {*|jQuery} Button
 */
function actionButton(text, type) {
	var id = text.replace(/ /g, '');
	if (!type)
		type = id;
	var btn = $('<button></button>')
		.attr('id', id)
		.addClass(type)
		.text(text)
		.appendTo($('#menu3').show());
	resize();
	return btn;
}

/**
 * Add a button to front of menu 3
 * @param text Button text
 * @returns {*|jQuery} Button
 */
function insertActionButton(text, type) {
	var id = text.replace(/ /g, '');
	if (!type)
		type = id;
	var btn = $('<button></button>')
		.attr('id', id)
		.addClass(type)
		.text(text)
		.prependTo($('#menu3').show());
	resize();
	return btn;
}

/**
 * Show message at top of screen
 * @param m message
 */
function message(m) {
	if (m) $('#message').text(m);
	else $('#message').html('&nbsp;');
}

/**
 * Layout the window after a resize
 */
function resize() {
	var top = $('#outerheader').height();
	// A small screen - should match "@media screen and (min-width:700px)" in default.css
	var auto = !$('#outerheader').is(':visible');
	$('#spacer').css('height', auto ? '' : top + 'px');
	$('#body').css('height', auto ? '' : ($(window).height() - top - 16) + 'px');
}

/**
 * Parse a decimal number (up to 2 places).
 * @param n Number string
 * @returns {*} Formatted number, or n if n is null, empty or 0
 */
function parseNumber(n) {
	if (!n)
		return n;
	if (!/^[+-]?\d+(\.\d{1,2})?$/.test(n))
		throw n + ' is not a number';
	return parseFloat(n);
}

/**
 * Parse a double number (up to 4 places).
 * @param n Number string
 * @returns {*} Formatted number, or n if n is null, empty or 0
 */
function parseDouble(n) {
	if (!n)
		return n;
	if (!/^[+-]?\d+(\.\d{1,4})?$/.test(n))
		throw n + ' is not a number';
	return parseFloat(n);
}

/**
 * Parse an intefer number.
 * @param n Number string
 * @returns {*} Formatted number, or n if n is null, empty or 0
 */
function parseInteger(n) {
	if (!n)
		return n;
	if (!/^[+-]?\d+$/.test(n))
		throw n + ' is not a whole number';
	return parseInt(n);
}

/**
 * Parse a date.
 * @param {string} date
 * @returns {Date|string} Date, or argument if it is null or empty
 */
function parseDate(date) {
	if (!date)
		return date;
	try {
		return new Date(Date.parse(date));
	} catch (e) {
		return date;
	}
}

/**
 * Format a date into local format.
 * @param {string|Date} date
 * @returns {string} Formatted date, or '' if invalid
 */
function formatDate(date, format) {
	if (!date)
		return date || '';
	try {
		var d = Date.parse(date);
		if (isNaN(d))
			return date || '';
		return $.datepicker.formatDate(format || dateStyle.dateFormat, new Date(d));
		// return new Date(d).toLocaleDateString(window.navigator.userLanguage || window.navigator.language);
	} catch (e) {
		return date || '';
	}
}

/**
 * Format a date for a date input field.
 * @param {string|Date} date
 * @returns {string} Formatted date, or '' if invalid
 */
function formatDateForInput(date) {
	return date ? date.substring(0, 10) : '';
}

/**
 * Format a date & time into local format.
 * @param {string} date
 * @returns {string} Formatted date, or '' if invalid
 */
function formatDateTime(date) {
	if (!date)
		return date;
	try {
		return new Date(Date.parse(date)).toLocaleString(window.navigator.userLanguage || window.navigator.language);
	} catch (e) {
		return date;
	}
}

/**
 * Format a decimal number to 2 places.
 * @param number
 * @returns {string}
 */
function formatNumber(number) {
	return number == null || number === '' ? '' : parseFloat(number).toFixed(2);
}

/**
 * Format a decimal number to 2 places with commas.
 * @param number
 * @returns {string}
 */
function formatNumberWithCommas(number) {
	if (number == null || number === '')
		return '';
	number = parseFloat(number).toLocaleString(window.navigator.userLanguage || window.navigator.language);
	var p = number.indexOf(decPoint);
	if (p == -1)
		return number + '.00';
	return (number + '00').substring(0, p + 3);
}

/**
 * Format an integer with commas.
 * @param number
 * @returns {string}
 */
function formatWholeNumberWithCommas(number) {
	number = formatNumberWithCommas(number);
	var p = number.indexOf(decPoint);
	if (p >= 0)
		number = number.substring(0, p);
	return number;
}

/**
 * Format a decimal number to 2 places with commas, and brackets if negative.
 * @param number
 * @returns {string}
 */
function formatNumberWithBrackets(number) {
	if (number == null || number === '')
		return '';
	number = formatNumberWithCommas(number);
	if (number[0] == '-')
		number = '(' + number.substring(1) + ')';
	else
		number += "\u00a0";
	return number;
}

/**
 * Format a double number with up to 4 places (no trailing zeroes after decimal point).
 * @param {number|string} number
 * @returns {string}
 */
function formatDouble(number) {
	if (number != null && number !== '') {
		number = parseFloat(number).toFixed(4);
		if (number.indexOf('.') >= 0) {
			var zeroes = /\.?0+$/.exec(number);
			if (zeroes) {
				number = number.substring(0, number.length - zeroes[0].length)
					+ '<span class="t">' + zeroes[0] + '</span>';
			}
		}
		return number;
	}
	return '';
}

/**
 * Format an integer
 * @param number
 * @returns {*}
 */
function formatInteger(number) {
	return number == null || number === '' ? '' : parseInt(number);
}

/**
 * Split a fractional number into 2 parts (e.g. Hours and Minutes)
 * @param {number} n number
 * @param {number} m Number of second part items in 1 first part item (e.g. Mins in an Hour)
 * @returns {*[]} The whole part, and the fractional part multiplied by m
 */
function splitNumber(n, m) {
	var sign = n < 0 ? -1 : 1;
	n = Math.abs(n);
	var w = Math.floor(n);
	return [sign * w, (n - w) * m];
}

/**
 * Add leading zeroes so result is 2 characters long
 * @param n
 * @returns {string}
 */
function fillNumber(n) {
	return ('00' + n).slice(-2);
}

/**
 * Convert a fractional number to one of our supported units for display
 * @param data The number
 * @param unit
 * @returns {string}
 */
function toUnit(data, unit) {
	if (data) {
		switch (unit) {
			case 1:	// D:H:M
				var d = splitNumber(data, 8);
				var m = splitNumber(d[1], 60);
				data = d[0] + ':' + m[0] + ':' + fillNumber(m[1]);
				break;
			case 2:	// H:M
				d = splitNumber(data, 60);
				data = d[0] + ':' + fillNumber(d[1]);
				break;
			case 3:
				data = parseFloat(data).toFixed(0);
				break;
			case 4:
				data = parseFloat(data).toFixed(1);
				break;
			case 5:
				data = parseFloat(data).toFixed(2);
				break;
			case 6:
				data = parseFloat(data).toFixed(3);
				break;
			case 7:
				data = parseFloat(data).toFixed(4);
				break;
			default:
				data = parseFloat(data).toFixed(4).replace(/\.?0+$/, '');
				break;
		}
	}
	return data;
}

/**
 * Convert an input unit into a fractional number
 * @param data
 * @param unit
 * @returns {number}
 */
function fromUnit(data, unit) {
	if (data) {
		switch (unit) {
			case 0:
				data = parseDouble(data);
				break;
			case 1:	// D:H:M
				var parts = data.split(':');
				switch (parts.length) {
					case 1:
						data = parseDouble(parts[0]);
						break;
					case 2:
						data = parseDouble(parts[0]) + parseDouble(parts[1]) / 8;
						break;
					case 3:
						data = parseDouble(parts[0]) + parseDouble(parts[1]) / 8 + parseDouble(parts[2]) / 480;
						break;
					default:
						throw data + ' is not in the format D:H:M';
				}
				break;
			case 2:	// H:M
				parts = data.split(':');
				switch (parts.length) {
					case 1:
						data = parseDouble(parts[0]);
						break;
					case 2:
						data = parseDouble(parts[0]) + parseDouble(parts[1]) / 60;
						break;
					case 3:
						data = parseFloat(parts[0]) * 8 + parseDouble(parts[1]) + parseDouble(parts[2]) / 60;
						break;
					default:
						throw data + ' is not in the format (D:)H:M';
				}
				break;
			case 3:
				data = parseInteger(data);
				break;
		}
	}
	return data;
}

//noinspection JSUnusedLocalSymbols,JSUnusedLocalSymbols
/**
 * Form field types
 * Each type has the following optional members (which may be overridden in the field definition):
 * render(data, type, row, meta): Renders - see DataTable documentation
 * 		data: The data for the field
 * 		type: The render type
 * 		row: The data for the whole row
 * 		meta: Information about the field
 * draw(data, rowno, row): Renders for display
 * 		data: The data for the field
 * 		rowno: The row number
 * 		row: The data for the whole row
 * defaultContent(index, col): Html to display if there is no data yet
 * update(cell, data, rowno, row): Update the table cell with the current value of data
 * 		cell: Table cell
 * 		data: The data for the field
 * 		rowno: The row number
 * 		row: The data for the whole row
 * inputValue(field, row): Extract the input value from what the user typed in the field
 * 		field: JQuery selector of the input field
 * 		row: The data for the whole row
 * sClass: The css class
 * name: The field name
 * heading: The field heading
 * selectOptions: Array of options for selects
 *
 * If any of the above are not supplied, suitable defaults are created
 */
var Type = {
	// Display only fields
	string: {
	},
	date: {
		render: function (data, type, row, meta) {
			switch (type) {
				case 'display':
				case 'filter':
					return formatDate(data);
				default:
					return data ? data.substring(0, 10) : data;
			}
		},
		download: function (data, rowno, row) {
			return formatDate(data);
		}
	},
	dateTime: {
		download: function (data, rowno, row) {
			return formatDateTime(data);
		}
	},
	decimal: {
		render: {
			display: formatNumberWithCommas,
			filter: formatNumber
		},
		draw: formatNumberWithCommas,
		download: formatNumber,
		sClass: 'n'
	},
	wholeDecimal: {
		render: {
			display: formatWholeNumberWithCommas,
			filter: formatNumber
		},
		draw: formatWholeNumberWithCommas,
		download: formatNumber,
		sClass: 'n'
	},
	bracket: {
		render: {
			display: formatNumberWithBrackets,
			filter: formatNumber
		},
		draw: formatNumberWithBrackets,
		download: formatNumber,
		sClass: 'n'
	},
	double: {
		download: formatDouble,
		sClass: 'n'
	},
	amount: {
		render: numberRender,
		draw: function (data, rowno, row) {
			return formatNumberWithCommas(Math.abs(data));
		},
		download: formatNumber,
		sClass: 'n'
	},
	credit: {
		// Displays abs value only if negative
		name: 'Credit',
		heading: 'Credit',
		render: numberRender,
		draw: function (data, rowno, row) {
			if (row["Credit"] !== undefined)
				data = -row["Credit"];
			return data < 0 ? formatNumberWithCommas(-data) : '';
		},
		download: function (data, rowno, row) {
			if (row["Credit"] !== undefined)
				data = -row["Credit"];
			return data < 0 ? formatNumber(-data) : '';
		},
		sClass: 'n'
	},
	debit: {
		// Displays value only if positive (or 0)
		name: 'Debit',
		heading: 'Debit',
		render: numberRender,
		draw: function (data, rowno, row) {
			if (row["Debit"] !== undefined)
				data = row["Debit"];
			return data >= 0 ? formatNumberWithCommas(data) : '';
		},
		download: function (data, rowno, row) {
			if (row["Debit"] !== undefined)
				data = row["Debit"];
			return data >= 0 ? formatNumber(data) : '';
		},
		sClass: 'n'
	},
	int: {
		download: formatInteger,
		sClass: 'n'
	},
	email: {
		draw: function (data, rowno, row) {
			if (!data)
				return '';
			return new SafeHtml('a', { href: 'mailto:' + data, text: data }).html();
		}
	},
	checkbox: {
		defaultContent: function (index, col) {
			return new SafeHtml('img', col, {
				src: '/images/untick.png'
			}).html();
		},
		draw: function (data, rowno, row) {
			return new SafeHtml('img', this, rowno, {
				src: data ? '/images/tick.png' : '/images/untick.png'
			}).html();
		}
	},
	select: {
		// Displays appropriate text from selectOptions according to value
		draw: function (data, rowno, row) {
			if (this.selectOptions) {
				var opt = _.find(this.selectOptions, function (o) { return o.id == data; });
				if (opt)
					return new SafeHtml('span', {
						title: opt.hint,
						text: opt.value
					}).html();
			}
			return _.escape(data);
		},
		download: function (data, rowno, row) {
			if (this.selectOptions) {
				var opt = _.find(this.selectOptions, function (o) { return o.id == data; });
				if (opt)
					data = opt.value;
			}
			return new SafeHtml().text(data).html();
		}
	},
	image: {
		defaultContent: function (index, col) {
			return new SafeHtml('img', col).html();
		},
		draw: function (data, rowno, row) {
			return new SafeHtml('img', this, rowno, {
				src: data
			}).html();
		}
	},
	inlineImage: {
		// Base64 encoded Image
		defaultContent: function (index, col) {
			var result = new SafeHtml();
			result.add('img', col, );
			return result.html();
		},
		draw: function (data, rowno, row) {
			if (data == null)
				data = '';
			var result = new SafeHtml();
			result.add('img', this, rowno, {
				src: data
			});
			return result.html();
		},
	},
	inlineImageInput: {
		// Base64 encoded Image
		defaultContent: function (index, col) {
			var result = new SafeHtml();
			var div = result.add('div', col, {
				"class": "inlineImageInput"
			});
			div.add('img');
			div.add('br');
			result.addTo(div, 'input', col, {
				type: 'file'
			});
			return result.html();
		},
		draw: function (data, rowno, row) {
			if (data == null)
				data = '';
			var result = new SafeHtml();
			var div = result.add('div', {
				"class": "inlineImageInput"
			});
			result.addTo(div, 'img', {
				src: data
			});
			result.addTo(div, 'input', {
				type: 'file'
			});
			result.addTo(div, 'input', this, rowno, {
				type: 'text',
				class: 'imageDummy',
				value: data
			});
			return result.html();
		},
		update: function (cell, data, rowno, row) {
			if (data == null)
				data = '';
			var i = cell.find('img');
			if (i.length && i.attr('id')) {
				i.prop('src', data);
			} else {
				cell.html(this.draw(data, rowno, row));
			}
			i = cell.find('input.imageDummy');
			if (i.length)
				i.val(data);
		}
	},
	textArea: {
		draw: function (data, rowno, row) {
			if (data == null)
				data = "";
			return new SafeHtml('div', this, rowno, {
				"class": 'prewrap',
				text: data
			}).html();
		}
	},
	colour: {
		draw: function (data, rowno, row) {
			if (data == null)
				data = "";
			var result = new SafeHtml('div', this, rowno, {
				style: "background-color: " + data,
				text: data
			});
			return result.html();
		}
	},
	autoComplete: {
		// Auto complete input field
		defaultContent: function (index, col, row) {
			if (col.confirmAdd || col.mustExist) {
				// Prompt user if value doesn't already exist in selectOptions
				//noinspection JSUnusedLocalSymbols
				col.change = function (newValue, rowData, col, input) {
					var item = _.find(col.selectOptions, function (v) {
						return v.value == newValue;
					});
					if (item === undefined) {
						if (col.mustExist) {
							message('You must choose an existing ' + col.heading);
							return false;
						}
						if (confirm(col.heading + ' ' + newValue + ' not found - add')) {
							item = {
								id: 0,
								value: newValue
							};
							col.selectOptions.push(item);
						} else {
							return false;
						}
					}
				};
			}
			return new SafeHtml('input', col, {
				type: 'text',
				"class": 'autoComplete'
			}).html();
		},
		draw: function (data, rowno, row) {
			return new SafeHtml('input', this, rowno, {
				type: 'text',
				"class": 'autoComplete',
				value: data
			}).html();
		},
		update: function (cell, data, rowno, row) {
			if (data == null)
				data = this && this.emptyValue != null ? this.emptyValue : '';
			var value = _.find(this.selectOptions, function (v) { return v.id == data; });
			if (value && value.value !== undefined)
				data = value.value;
			var i = cell.find('input');
			if (i.length && i.attr('id')) {
				i.val(data);
			} else {
				cell.html(this.draw(data, rowno, row));
				i = cell.find('input');
			}
			if (i.hasClass('ui-autocomplete-input'))
				return;
			var self = this;
			//noinspection JSUnusedLocalSymbols
			var options = {
				source: function (req, resp) {
					var re = $.ui.autocomplete.escapeRegex(req.term);
					var matcher = new RegExp((self.matchBeginning ? '^' : '') + re, "i");
					resp(_.filter(self.selectOptions, function (o) {
						return !o.hide && matcher.test(o.value);
					}));
				},
				change: function (e) {
					$(this).trigger('change');
				}
			};
			if (this.autoFill) {
				options.minLength = 2;
				options.open = function (event, ui) {
					var firstElement = $(this).data("uiAutocomplete").menu.element[0].children[0],
						inpt = $(this),
						original = inpt.val(),
						firstElementText = $(firstElement).text();

					/*
					   here we want to make sure that we're not matching something that doesn't start
					   with what was typed in
					*/
					if (firstElementText.toLowerCase().indexOf(original.toLowerCase()) === 0) {
						inpt.val(firstElementText);//change the input to the first match

						inpt[0].selectionStart = original.length; //highlight from end of input
						inpt[0].selectionEnd = firstElementText.length;//highlight to the end
					}
				};
			};
			if ($.isArray(this.selectOptions) && this.selectOptions.length > 0 && this.selectOptions[0].category != null) {
				i.catcomplete(options);
			} else {
				i.autocomplete(options);
			}
		}
	},
	autoDropdown: {
		defaultContent: function (index, col, row) {
			return Type.autoComplete.defaultContent.call(this, index, col, row) + '<img class="autoDropdown" src="/images/down.png" />';
		},
		draw: function (data, rowno, row) {
			return Type.autoComplete.draw.call(this, data, rowno, row) + '<img class="autoDropdown" src="/images/down.png" />';
		},
		update: function (cell, data, rowno, row) {
			Type.autoComplete.update.call(cell, data, rowno, row);
		}
	},
	textInput: {
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'text',
				size: col.size ? col.size : col.maxlength,
				maxlength: col.maxlength
			}).html();
		},
		draw: function (data, rowno, row) {
			if (data == null)
				data = "";
			return new SafeHtml('input', this, rowno, {
				type: 'text',
				size: this.size ? this.size : this.maxlength,
				maxlength: this.maxlength,
				value: data
			}).html();
		},
		update: function (cell, data, rowno, row) {
			colUpdate('input', cell, data, rowno, this, row);
		},
		maxlength: 45
	},
	docIdInput: {
		// Document number - also add a "Next" button to set value to <Next>
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'text',
				size: col.size ? col.size : col.maxlength,
				maxlength: col.maxlength
			}).html();
		},
		draw: function (data, rowno, row) {
			if (data == null)
				data = "";
			var result = new SafeHtml('input', this, rowno, {
				type: 'text',
				size: this.size ? this.size : this.maxlength,
				maxlength: this.maxlength,
				value: data
			});
			if (row.idDocument !== undefined && !row.idDocument)
				result.add('button', {
					"class": 'nextButton',
					text: 'Next'
				});
			return result.html();
		},
		update: function (cell, data, rowno, row) {
			colUpdate('input', cell, data, rowno, this, row);
		},
		maxlength: 45
	},
	passwordInput: {
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'password',
				size: col.size ? col.size : col.maxlength,
				maxlength: col.maxlength
			}).html();
		},
		draw: function (data, rowno, row) {
			if (data == null)
				data = "";
			return new SafeHtml('input', this, rowno, {
				type: 'password',
				size: this.size ? this.size : this.maxlength,
				maxlength: this.maxlength,
				value: data
			}).html();
		},
		update: function (cell, data, rowno, row) {
			colUpdate('input', cell, data, rowno, this, row);
		},
		maxlength: 45
	},
	textAreaInput: {
		defaultContent: function (index, col) {
			var rows = col.rows || 6;
			var cols = col.cols || 50;
			return new SafeHtml('textarea', col, {
				rows: rows,
				cols: cols,
				maxlength: col.maxlength
			}).html();
		},
		draw: function (data, rowno, row) {
			if (data == null)
				data = "";
			var rows = this.rows || 5;
			var cols = this.cols || 50;
			return new SafeHtml('textarea', this, rowno, {
				rows: rows,
				cols: cols,
				maxlength: this.maxlength,
				text: data
			}).html();
		},
		update: function (cell, data, rowno, row) {
			colUpdate('textarea', cell, data, rowno, this, row);
		}
	},
	dateInput: {
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'date',
				"class": 'date'
			}).html();
		},
		draw: function (data, rowno, row) {
			data = formatDateForInput(data);
			return new SafeHtml('input', this, rowno, {
				type: 'date',
				"class": 'date',
				value: data
			}).html();
		},
		update: function (cell, data, rowno, row) {
			data = formatDateForInput(data);
			colUpdate('input', cell, data, rowno, this, row);
		}
	},
	decimalInput: {
		// 2 dec places
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'number',
				step: '0.01'
			}).html();
		},
		draw: function (data, rowno, row) {
			return new SafeHtml('input', this, rowno, {
				type: 'number',
				step: '0.01',
				value: formatNumber(data)
			}).html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('input');
			if (i.length && i.attr('id')) {
				i.val(formatNumber(data));
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		inputValue: function (field, row) {
			return parseNumber($(field).val());
		},
		sClass: 'ni'
	},
	doubleInput: {
		// Up to 4 dec places
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, { type: 'text', value: 0 }).html();
		},
		draw: function (data, rowno, row) {
			return new SafeHtml('input', this, rowno, {
				type: 'text',
				value: toUnit(data, row.Unit)
			}).html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('input');
			if (i.length && i.attr('id')) {
				i.val(toUnit(data, row.Unit));
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		inputValue: function (field, row) {
			return fromUnit($(field).val(), row.Unit);
		},
		maxlength: 7,
		sClass: 'ni'
	},
	creditInput: {
		name: 'Credit',
		heading: 'Credit',
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'number',
				step: '0.01',
				value: '0.00'
			}).html();
		},
		draw: function (data, rowno, row) {
			data = data < 0 ? formatNumber(-data) : '';
			return new SafeHtml('input', this, rowno, {
				type: 'number',
				step: '0.01',
				value: data
			}).html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('input');
			if (i.length && i.attr('id')) {
				i.val(data < 0 ? formatNumber(-data) : '');
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		inputValue: function (field, row) {
			return parseNumber($(field).val()) * -1;
		},
		sClass: 'ni'
	},
	debitInput: {
		name: 'Debit',
		heading: 'Debit',
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'number',
				step: '0.01',
				value: '0.00'
			}).html();
		},
		draw: function (data, rowno, row) {
			data = data >= 0 ? formatNumber(data) : '';
			return new SafeHtml('input', this, rowno, {
				type: 'number',
				step: '0.01',
				value: data
			}).html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('input');
			if (i.length && i.attr('id')) {
				i.val(data >= 0 ? formatNumber(data) : '');
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		inputValue: function (field, row) {
			return parseNumber($(field).val());
		},
		sClass: 'ni'
	},
	intInput: {
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'number',
				step: '1',
				value: '0'
			}).html();
		},
		draw: function (data, rowno, row) {
			return new SafeHtml('input', this, rowno, {
				type: 'number',
				step: '1',
				value: formatInteger(data)
			}).html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('input');
			if (i.length && i.attr('id')) {
				i.val(formatInteger(data));
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		inputValue: function (field, row) {
			return parseInteger($(field).val());
		},
		sClass: 'ni'
	},
	checkboxInput: {
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'checkbox'
			}).html();
		},
		draw: function (data, rowno, row) {
			return new SafeHtml('input', this, rowno, {
				type: 'checkbox',
				checked: data ? true : false
			}).html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('input');
			if (i.length && i.attr('id')) {
				i.prop('checked', data ? true : false);
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		inputValue: function (field, row) {
			return $(field).prop('checked') ? 1 : 0;
		}
	},
	bitFlag: {
		defaultContent: function (index, col) {
			return new SafeHtml('img', col, {
				src: '/images/untick.png'
			}).html();
		},
		draw: function (data, rowno, row) {
			var select = new SafeHtml();
			if (this.selectOptions) {
				_.each(this.selectOptions, function (o) {
					if (!o.id)
						return;
					var l = select.add('label');
					select.addTo(l, 'img', this, rowno, {
						src: '/images/' + ((o.id & data) == o.id ? 'tick' : 'untick') + '.png'
					});
					l.append(_.escape(o.value));
				});
			}
			return select.html();
		}
	},
	bitFlagInput: {
		draw: function (data, rowno, row) {
			var select = new SafeHtml();
			var self = this;
			if (this.selectOptions) {
				_.each(this.selectOptions, function (o) {
					if (!o.id)
						return;
					var l = select.add('label');
					select.addTo(l, 'input', self, rowno, {
						type: 'checkbox',
						value: o.id,
						checked: (o.id & data) == o.id
					});
					l.append(_.escape(o.value));
				});
			}
			return select.html();
		},
		update: function (cell, data, rowno, row) {
			if (cell.find('input#r' + rowno + 'c' + this.name).length == 0) {
				cell.html(this.draw(data, rowno, row));
			} else {
				if (this.selectOptions) {
					_.each(this.selectOptions, function (o) {
						var i = cell.find('input[type=checkbox][value=' + o.id + ']');
						if (i.length)
							i.prop('checked', (o.id & data) == o.id);
					});
				}
			}
		},
		inputValue: function (field, row) {
			var value = 0;
			var cell = $(field).closest('td');
			cell.find('input').each(function () {
				if ($(this).prop('checked'))
					value |= parseInt(this.value);
			});
			return value;
		}
	},
	imageInput: {
		// Image file, with auto upload
		defaultContent: function (index, col) {
			var result = new SafeHtml();
			result.add('img');
			result.add('br');
			result.add('input', col, {
				type: 'file',
				"class": 'autoUpload'
			});
			return result.html();
		},
		draw: function (data, rowno, row) {
			if (data == null)
				data = '';
			var result = new SafeHtml();
			result.add('img', this, rowno, {
				src: data
			});
			result.add('br');
			result.add('input', this, rowno, {
				type: 'file',
				"class": 'autoUpload',
				multiple: this.multiple
			});
			return result.html();
		},
		update: function (cell, data, rowno, row) {
			if (data == null)
				data = '';
			var i = cell.find('img');
			if (i.length && i.attr('id')) {
				i.prop('src', data);
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		}
	},
	file: {
		// File input, no auto upload
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'file'
			}).html();
		},
		draw: function (data, rowno, row) {
			return new SafeHtml('input', this, rowno, {
				type: 'file',
				multiple: this.multiple
			}).html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('input#r' + rowno + 'c' + this.name);
			if (!i.length)
				cell.html(this.draw(data, rowno, row));
		}
	},
	radioInput: {
		// Radio buttons from select options
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'radio',
				value: '0'
			}).html();
		},
		draw: function (data, rowno, row) {
			var select = new SafeHtml();
			var self = this;
			if (this.selectOptions) {
				_.each(this.selectOptions, function (o) {
					var s = select.add('span', { text: ' ' });
					var l = select.addTo(s, 'label');
					var b = select.addTo(l, 'input', self, rowno, {
						type: 'radio',
						value: o.id,
						name: 'r' + rowno + 'c' + self.name,
						checked: o.id == data
					});
					l.append(_.escape(o.value));
					if (o.hint) {
						s.attr('title', o.hint);
						s.append(' <span class="hint">?</span>');
					}
				});
			} else
				select.addTo(select.add('label', {
					text: 'Other'
				}), 'input', this, rowno, {
					type: 'radio',
					value: '0',
					name: 'r' + rowno + 'c' + self.name
				});
			return select.html();
		},
		update: function (cell, data, rowno, row) {
			if (cell.find('input#r' + rowno + 'c' + this.name).length == 0) {
				cell.html(this.draw(data, rowno, row));
			} else {
				var i = cell.find('input[type=radio][value=' + data + ']');
				if (i.length)
					i.prop('checked', true);
			}
		},
		inputValue: function (field, row) {
			return field.value;
		}

	},
	textAreaField: {
		defaultContent: function (index, col) {
			var rows = col.rows || 6;
			var cols = col.cols || 50;
			return new SafeHtml('textarea', col, {
				rows: rows,
				cols: cols,
				maxlength: col.maxlength,
				disabled: true
			}).html();
		},
		draw: function (data, rowno, row) {
			if (data == null)
				data = "";
			var rows = this.rows || 5;
			var cols = this.cols || 50;
			return new SafeHtml('textarea', this, rowno, {
				rows: rows,
				cols: cols,
				maxlength: this.maxlength,
				text: data,
				disabled: true
			}).html();
		},
		update: function (cell, data, rowno, row) {
			colUpdate('textarea', cell, data, rowno, this, row);
		}
	},
	decimalField: {
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'number',
				step: '0.01',
				disabled: true
			}).html();
		},
		draw: function (data, rowno, row) {
			return new SafeHtml('input', this, rowno, {
				type: 'number',
				step: '0.01',
				value: formatNumber(data),
				disabled: true
			}).html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('input');
			if (i.length && i.attr('id')) {
				i.val(formatNumber(data));
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		sClass: 'ni'
	},
	doubleField: {
		defaultContent: function (index, col) {
			return new SafeHtml('input', col, {
				type: 'text',
				value: 0,
				disabled: true
			}).html();
		},
		draw: function (data, rowno, row) {
			return new SafeHtml('input', this, rowno, {
				type: 'text',
				value: toUnit(data, row.Unit),
				disabled: true
			}).html();
		},
		update: function (cell, data, rowno, row) {
			colUpdate('input', cell, data, rowno, this, row);
		},
		sClass: 'ni'
	},
	selectInput: {
		defaultContent: function (index, col) {
			return new SafeHtml('select', col).html();
		},
		draw: function (data, rowno, row) {
			var result = new SafeHtml();
			var select = result.add('select', this, rowno);
			if (this.selectOptions)
				addOptionsToSelect(select, this.selectOptions, data, this);
			return result.html();
		},
		update: function (cell, data, rowno, row) {
			colUpdate('select', cell, data, rowno, this, row);
		}
	},
	selectFilter: {
		// Report filter
		defaultContent: function (index, col) {
			return new SafeHtml('select', col).html();
		},
		draw: function (data, rowno, row) {
			var result = new SafeHtml();
			var select = result.add('select', this, rowno);
			if (this.selectOptions)
				addOptionsToSelect(select, this.selectOptions, data, this);
			return result.html();
		},
		update: function (cell, data, rowno, row) {
			colUpdate('select', cell, data, rowno, this, row);
		}
	},
	dateFilter: {
		// Report date filter
		defaultContent: function (index, col) {
			var result = new SafeHtml('select', col);
			var nobr = result.add('nobr');
			result.addTo(nobr, 'input', col, { type: 'number' });
			result.addTo(nobr, 'input', col, { type: 'date' });
			nobr.append(' - ');
			result.addTo(nobr, 'input', col, { type: 'date' });
			return result.html();
		},
		draw: function (data, rowno, row) {
			var range = data.range || 4;
			var count = data.count ? data.count : '';
			var start = data.start ? data.start.substring(0, 10) : '';
			var end = data.end ? data.end.substring(0, 10) : '';
			var result = new SafeHtml();
			var select = result.add('select', this, rowno, 'r');
			addOptionsToSelect(select, dateSelectOptions, range);
			var nobr = result.add('nobr');
			result.addTo(nobr, 'input', this, rowno, 'c', {
				type: 'number',
				value: count,
				disabled: range <= 12
			});
			result.addTo(nobr, 'input', this, rowno, 's', {
				type: 'date',
				value: start,
				disabled: range != 12
			});
			nobr.append(' - ');
			result.addTo(nobr, 'input', this, rowno, 'e', {
				type: 'date',
				value: end,
				disabled: range != 12
			});
			return result.html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('select');
			var range = data.range || 4;
			var count = data.count ? data.count : '';
			var start = data.start ? data.start.substring(0, 10) : '';
			var end = data.end ? data.end.substring(0, 10) : '';
			if (i.length && i.attr('id')) {
				i.val(range);
				i = cell.find('input');
				i.prop('disabled', range != 12);
				$(i[0]).prop('disabled', range <= 12);
				$(i[0]).val(count);
				$(i[1]).val(start);
				$(i[2]).val(end);
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		inputValue: function (field, row) {
			var cell = $(field).closest('td');
			var range = cell.find('select').val();
			i = cell.find('input');
			i.prop('disabled', range != 12);
			$(i[0]).prop('disabled', range <= 12);
			return {
				range: range,
				count: $(i[0]).val(),
				start: $(i[1]).val(),
				end: $(i[2]).val()
			};
		}
	},
	multiSelectFilter: {
		// Report multi select filter
		defaultContent: function (index, col) {
			return new SafeHtml('select', col, { multiple: true }).html();
		},
		draw: function (data, rowno, row) {
			var result = new SafeHtml();
			var select = result.add('select', this, rowno, { multiple: true });
			if (this.selectOptions) {
				addOptionsToSelect(select, this.selectOptions, data, this);
				select.val(data);
			}
			return result.html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('select');
			if (i.length && i.attr('id')) {
				i.val(data);
			} else {
				cell.html(this.draw(data, rowno, row));
				i = cell.find('select');
			}
			if (i.css('display') != 'none')
				i.multiselect({
					selectedList: 2,
					uncheckAllText: 'No filter',
					noneSelectedText: 'No filter'
				});
		}
	},
	decimalFilter: {
		// Report decimal filter
		defaultContent: function (index, col) {
			var result = new SafeHtml();
			result.add('select', col);
			result.add('input', col, {
				type: 'number',
				step: '0.01',
				value: '0.00'
			});
			return result.html();
		},
		draw: function (data, rowno, row) {
			var comparison = data.comparison || 0;
			var value = data.value || 0;
			var result = new SafeHtml();
			var select = result.add('select', this, rowno, 'r');
			addOptionsToSelect(select, decimalSelectOptions, comparison);
			result.add('input', this, rowno, {
				type: 'number',
				step: '0.01',
				value: formatNumber(value),
				disabled: comparison <= 2
			});
			return result.html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('select');
			var comparison = data.comparison || 0;
			var value = data.value || 0;
			if (i.length && i.attr('id')) {
				i.val(comparison);
				i = cell.find('input');
				i.prop('disabled', comparison <= 2);
				i.val(value);
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		inputValue: function (field, row) {
			var cell = $(field).closest('td');
			var comparison = cell.find('select').val();
			cell.find('input').prop('disabled', comparison <= 2);
			return {
				comparison: comparison,
				value: parseNumber(cell.find('input').val())
			};
		}
	},
	doubleFilter: {
		// Report double (up tp 4 places) filter
		defaultContent: function (index, col) {
			var result = new SafeHtml();
			result.add('select', col);
			result.add('input', col, {
				type: 'number',
				value: '0'
			});
			return result.html();
		},
		draw: function (data, rowno, row) {
			var comparison = data.comparison || 0;
			var value = data.value || 0;
			var result = new SafeHtml();
			var select = result.add('select', this, rowno, 'r');
			addOptionsToSelect(select, decimalSelectOptions, comparison);
			result.add('input', this, rowno, {
				type: 'number',
				value: value,
				disabled: comparison <= 2
			});
			return result.html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('select');
			var comparison = data.comparison || 0;
			var value = data.value || 0;
			if (i.length && i.attr('id')) {
				i.val(comparison);
				i = cell.find('input');
				i.prop('disabled', comparison <= 2);
				i.val(value);
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		inputValue: function (field, row) {
			var cell = $(field).closest('td');
			var comparison = cell.find('select').val();
			cell.find('input').prop('disabled', comparison <= 2);
			return {
				comparison: comparison,
				value: parseDouble(cell.find('input').val())
			};
		}
	},
	stringFilter: {
		// Report string filter
		defaultContent: function (index, col) {
			var result = new SafeHtml();
			result.add('select', col);
			result.add('input', col, { type: 'text' });
			return result.html();
		},
		draw: function (data, rowno, row) {
			var comparison = data.comparison || 0;
			var value = data.value || '';
			var result = new SafeHtml();
			var select = result.add('select', this, rowno, 'r');
			addOptionsToSelect(select, stringSelectOptions, comparison);
			result.add('input', this, rowno, {
				type: 'text',
				value: value,
				disabled: comparison <= 2
			});
			return result.html();
		},
		update: function (cell, data, rowno, row) {
			var i = cell.find('select');
			var comparison = data.comparison || 0;
			var value = data.value || '';
			if (i.length && i.attr('id')) {
				i.val(comparison);
				i = cell.find('input');
				i.prop('disabled', comparison <= 2);
				i.val(value);
			} else {
				cell.html(this.draw(data, rowno, row));
			}
		},
		inputValue: function (field, row) {
			var cell = $(field).closest('td');
			var comparison = cell.find('select').val();
			cell.find('input').prop('disabled', comparison <= 2);
			return {
				comparison: comparison,
				value: cell.find('input').val()
			};
		}
	}
};

/**
 * Report date selection options
 */
var dateSelectOptions = [
	{ id: 1, value: 'All' },
	{ id: 2, value: 'Today' },
	{ id: 3, value: 'This Week' },
	{ id: 4, value: 'This Month' },
	{ id: 5, value: 'This Quarter' },
	{ id: 6, value: 'This Year' },
	{ id: 7, value: 'Yesterday' },
	{ id: 8, value: 'Last Week' },
	{ id: 9, value: 'Last Month' },
	{ id: 10, value: 'Last Quarter' },
	{ id: 11, value: 'Last Year' },
	{ id: 12, value: 'Custom' },
	{ id: 13, value: 'Last N Days' },
	{ id: 14, value: 'Last N months' }

];

/**
 * Report decimal selection options
 */
var decimalSelectOptions = [
	{ id: 0, value: 'All' },
	{ id: 1, value: 'Zero' },
	{ id: 2, value: 'Non-zero' },
	{ id: 3, value: 'Less than or equal' },
	{ id: 4, value: 'Greater than or equal' },
	{ id: 5, value: 'Equal' },
	{ id: 6, value: 'Not equal' }

];

/**
 * Report string selection options
 */
var stringSelectOptions = [
	{ id: 0, value: 'All' },
	{ id: 1, value: 'Empty' },
	{ id: 2, value: 'Non-empty' },
	{ id: 3, value: 'Equal' },
	{ id: 4, value: 'Contains' },
	{ id: 5, value: 'Starts with' },
	{ id: 6, value: 'Ends with' }
];

/**
 * Units
 */
var unitOptions = [
	{ id: 0, value: 'decimal', unit: '' },
	{ id: 1, value: 'days', unit: 'D:H:M' },
	{ id: 2, value: 'hours', unit: 'H:M' },
	{ id: 3, value: 'units', unit: '' },
	{ id: 4, value: '1 dp', unit: '' },
	{ id: 5, value: '2 dp', unit: '' },
	{ id: 6, value: '3 dp', unit: '' },
	{ id: 7, value: '4 dp', unit: '' }
];

/**
 * Descriptions of units to show in unit column
 */
var unitDisplay = [
	{ id: 0, value: '' },
	{ id: 1, value: 'D:H:M' },
	{ id: 2, value: 'H:M' },
	{ id: 3, value: '' }
];

var DataTable;
var Forms = [];
/**
 * Make a DataTable
 * Column options are:
 * {string} [prefix]dataItemName[/heading] (prefix:#=decimal, /=date, @=email)
 * or
 * {*} [type] Type.* - sets defaults for column options
 * {string} data item name
 * {string} [heading]
 * {boolean|*}	nonZero true to suppress zero items, with button to reveal, false opposite, or:
 * 	{boolean} [hide] true to suppress zero items, with button to reveal, false opposite
 * 	{string} [heading] to use in button text (col.heading)
 * 	{string} [zeroText] prompt for button (Show all <heading>)
 * 	{string} [nonZeroText] prompt for button (Only non-zero <heading>)
 * 	{string} [regex] regex matching data to hide (^([0\.]*|true|null)$) - NB: default hides ticked checkboxes
 * @param {string} selector
 * @param options
 * @param {string} [options.table] Name of SQL table
 * @param {string} [options.idName] Name of id field in table (id<table>)
 * @param {string|function} [options.select] Url to go to or function to call when a row is clicked
 * @param {string|*} [options.ajax] Ajax settings, or string url (current url + 'Listing')
 * @param {number?} [options.iDisplayLength] Number of items to display per screen
 * @param {Array} [options.order] Initial sort order (see jquery.datatables)
 * @param {boolean?} [options.stateSave] Whether to save state
 * @param {function} [options.stateSaveCallback] Callback to save state
 * @param {function} [options.stateLoadCallback] Callback to load saved state
 * @param {*} [options.data] Existing data to display
 * @param {Array} options.columns
 * @param {function} [options.validate] Callback to validate data
 * @returns {*}
 */
function makeDataTable(selector, options) {
	var tableName = myOption('table', options);
	var idName = myOption('id', options, 'id' + tableName);
	var selectUrl = myOption('select', options);
	// Show All options
	var nzColumns = [];
	var dtParam = getParameter('dt');
	var nzList = dtParam === null || dtParam === '' ? [] : dtParam.split(',');
	// Default number of items to display depends on screen size
	if (options.iDisplayLength === undefined) {
		if ($(window).height() >= 1200)
			options.iDisplayLength = 25;
		if (localStorage) {
			var l = localStorage.getItem('iDisplayLength');
			if (l)
				options.iDisplayLength = l;
			$(body).on('change', 'div.dataTables_length select', function () {
				localStorage.setItem('iDisplayLength', $(this).val());
			});
		}
	}
	if (typeof (selectUrl) == 'string') {
		// Turn into a function that goes to url, adding id of current row as parameter
		var s = selectUrl;
		selectUrl = function (row, e) {
			goto(urlParameter('id', row[idName], s), e);
		};
	}
	// If no data or data url supplied, use an Ajax call to this method + "Listing"
	if (options.data === undefined)
		_setAjaxObject(options, 'Listing', '');
	$(selector).addClass('form');
	// Make sure there is a table heading
	var heading = $(selector).find('thead');
	if (heading.length == 0) heading = $('<thead></thead>').appendTo($(selector));
	heading = $('<tr></tr>').appendTo(heading);
	var columns = {};
	_.each(options.columns, function (col, index) {
		// Set up the column - add any missing functions, etc.
		options.columns[index] = col = _setColObject(col, tableName, index);
		var title = col.heading;
		var hdg = $('<th></th>').appendTo(heading).text(title).addClass(col.sClass).attr('title', col.hint);
		if (col.hint)
			hdg.append(' <span class="hint">?</span>');
		// Add to columns hash by name
		columns[col.name] = col;
		// "Show All" option?
		var nz = myOption('nonZero', col);
		if (nz != undefined) {
			if (typeof (nz) == 'boolean') nz = { hide: nz };
			nz.col = col;
			if (nz.hide === undefined) nz.hide = true;
			if (nz.heading === undefined) nz.heading = title;
			if (nz.zeroText === undefined) nz.zeroText = (col.type == 'checkbox' ? 'Exclude ' : 'Only non-zero ') + nz.heading;
			if (nz.nonZeroText === undefined) nz.nonZeroText = (col.type == 'checkbox' ? 'Include ' : 'Show all ') + nz.heading;
			nz.regex = nz.regex === undefined ? /^([0\.]*|true|null)$/ : new RegExp(nz.regex);
			if (nzList.length)
				nz.hide = nzList.shift() == 1;
			nzColumns.push(nz);
		}
		col.index = index;
	});
	if (options.order == null)
		options.order = [];
	if (options.stateSave === undefined) {
		// By default, save and restore table UI state
		options.stateSave = true;
		options.stateLoadCallback = function (settings) {
			try {
				if (dtParam !== null)
					return JSON.parse(sessionStorage.getItem(
						'DataTables_' + settings.sInstance + '_' + location.pathname + '_' + getParameter('id')
					));
			} catch (e) {
			}
			return {};
		};
		options.stateSaveCallback = function (settings, data) {
			try {
				sessionStorage.setItem(
					'DataTables_' + settings.sInstance + '_' + location.pathname + '_' + getParameter('id'),
					JSON.stringify(data)
				);
			} catch (e) {
			}
		};
	}
	if (typeof (selectUrl) == 'function')
		$(selector).addClass('noselect');
	options.rowCallback = function (row, data) {
		if (data['@class'])
			$(row).addClass(data['@class']);
	}
	var table = $(selector).dataTable(options);
	if (options.responsive)
		table.closest('.datatableContainer').addClass('screenWidth');

	// Attach mouse handlers to each row
	if (typeof (selectUrl) == 'function') {
		selectClick(selector, function (e) {
			return selectUrl.call(this, table.rowData($(this)), e);
		});
	} else {
		selectClick(selector, null);
	}
	if (options.download || options.download === undefined) {
		actionButton('Download', 'download').click(function () {
			var data = downloadData(table.fields, table.api().data());
			download(this, data);
		});
	}
	// "Show All" functionality
	_.each(nzColumns, function (nz) {
		var zText = nz.zeroText;
		var nzText = nz.nonZeroText;
		//noinspection JSUnusedLocalSymbols
		$('<button></button>').attr('id', 'nz' + nz.col.name).attr('data-nz', nz.hide).insertBefore($(selector))
			.html(nz.hide ? nzText : zText)
			.click(function (e) {
				nz.hide = !nz.hide;
				$(this).attr('data-nz', nz.hide);
				$(this).html(nz.hide ? nzText : zText);
				table.api().draw();
			});
		$.fn.dataTable.ext.search.push(
			function (settings, dataArray, dataIndex, data) {
				return !nz.hide || !nz.regex.test(data[nz.col.data]);
			}
		);
		if (nz.hide)
			table.api().draw(false);
	});
	// Attach event handler to input fields
	$('body').off('change', selector + ' :input');
	$('body').on('change', selector + ' :input', function () {
		$('button#Back').text('Cancel');
		var col = table.fields[$(this).attr('data-col')];
		if (col) {
			var row = table.row(this);
			var data = row.data();
			var val;
			try {
				//noinspection JSCheckFunctionSignatures
				val = col.inputValue(this, row);
			} catch (e) {
				message(col.heading + ':' + e);
				$(this).focus();
				return;
			}
			setTimeout(function () {
				if ($(selector).triggerHandler('changed.field', [val, data, col, this]) != false) {
					data[col.data] = val;
				}
			}, 10);
		}
	});
	/**
	 * Return the tr row of item clicked on
	 * @param item
	 * @returns {*}
	 */
	table.row = function (item) {
		item = $(item);
		if (item.attr['tagName'] != 'tr') item = item.closest('tr');
		return table.api().row(item);
	};
	/**
	 * Refresh the row containing item without losing the focus
	 * @param item
	 */
	table.refreshRow = function (item) {
		var focus = $(':focus');
		var col = focus.closest('td').index();
		var row = focus.closest('tr').index();
		var refocus = focus.closest('table')[0] == table[0];
		table.row(item).invalidate().draw(false);
		if (refocus)
			table.find('tbody tr:eq(' + row + ') td:eq(' + col + ') :input').focus();
	};
	/**
	 * Refresh the whole table without losing the focus
	   */
	table.refresh = function () {
		var focus = $(':focus');
		var col = focus.closest('td').index();
		var row = focus.closest('tr').index();
		var refocus = focus.closest('table')[0] == table[0];
		table.api().draw(false);
		if (refocus)
			table.find('tbody tr:eq(' + row + ') td:eq(' + col + ') :input').focus();
	};
	/**
	 * Return the data for the row containing r
	 * @param r
	 * @returns {*}
	 */
	table.rowData = function (r) {
		return table.row(r).data();
	};
	/**
	 * When data has arrived, update the table
	 * @param data
	 */
	table.dataReady = function (data) {
		table.api().clear();
		table.api().rows.add(data);
		table.api().draw();
	};
	table.fields = columns;
	DataTable = table.api();
	Forms.push(table);
	return table;
}

/**
 * Make a form to edit a single record and post it back
 * Column options are:
 * {string} [prefix]dataItemName[/heading] (prefix:#=decimal, /=date, @=email)
 * or
 * {*} [type] Type.* - sets defaults for column options
 * {string} data item name
 * {string} [heading]
 * @param {string} selector
 * @param options
 * @param {string} [options.table] Name of SQL table
 * @param {string} [options.idName] Name of id field in table (id<table>)
 * @param {string|function} [options.select] Url to go to or function to call when a row is clicked
 * @param {string|*} [options.ajax] Ajax settings, or string url (current url + 'Data')
 * @param {boolean} [options.dialog] Show form as a dialog when Edit button is pushed
 * @param {string} [options.submitText} Text to use for Save buttons (default "Save")
 * @param {boolean} [options.readonly} No save (default false)
 * @param {boolean} [options.apply} Include Apply button (default false)
 * @param {string} [options.applyText} Text to use for Apply button (default "Apply")
 * @param {boolean} [options.saveAndNew} Include Save and New button (default false)
 * @param {*} [options.data] Existing data to display
 * @param {Array} options.columns
 * @param {function} [options.validate] Callback to validate data
 * @returns {*}
 */
function makeForm(selector, options) {
	var tableName = myOption('table', options);
	var idName = myOption('id', options, 'id' + tableName);
	var canDelete = myOption('canDelete', options);
	var submitUrl = myOption('submit', options);
	var deleteButton;
	if (submitUrl === undefined) {
		submitUrl = defaultUrl('Save');
	}
	if (typeof (submitUrl) == 'string') {
		// Turn url into a function that validates and posts
		var submitHref = submitUrl;
		/**
		 * Submit method attached to button
		 * @param button The button pushed
		 */
		submitUrl = function (button, callback) {
			if (typeof (callback) != 'function')
				callback = null;
			var hdg = null;
			try {
				// Check each input value is valid
				_.each(options.columns, function (col) {
					if (col.inputValue) {
						hdg = col.heading;
						col.inputValue(col.cell.find('#r0c' + col.name), result.data);
					}
				});
			} catch (e) {
				message(hdg + ':' + e);
				return;
			}
			if (options.validate) {
				var msg = options.validate();
				message(msg);
				if (msg) return;
			}
			postJson(submitHref, result.data, function (d) {
				// Success
				$('button#Back').text('Back');
				if (callback && callback(d, true))
					return;
				if (result.submitCallback && result.submitCallback(d))
					return;
				if ($(button).hasClass('goback')) {
					goback();	// Save
				} else if ($(button).hasClass('new')) {
					window.location = urlParameter('id', 0);	// Save and New
				} else if (tableName && d.id) {
					window.location = urlParameter('id', d.id);	// Apply - redisplay saved record
				}
			}, function (d) {
				// Failure
				if (callback && callback(d, false))
					return;
				if (result.submitCallback)
					result.submitCallback(d);
			});
		};
	}
	var deleteUrl = canDelete ? myOption('delete', options) : null;
	if (deleteUrl === undefined) {
		deleteUrl = defaultUrl('Delete');
	}
	if (typeof (deleteUrl) == 'string') {
		var deleteHref = deleteUrl;
		//noinspection JSUnusedLocalSymbols
		deleteUrl = function (button) {
			postJson(deleteHref, result.data, goback);
		};
	}
	$(selector).addClass('form');
	_setAjaxObject(options, 'Data', '');
	var row, preambleRow, itemsInRow = 0;
	var columns = {};
	_.each(options.columns, function (col, index) {
		options.columns[index] = col = _setColObject(col, tableName, index);
		if (!row || !col.sameRow) {
			row = $('<tr class="form-question"></tr>').appendTo($(selector));
			preambleRow = null;
			itemsInRow = 0;
		} else if (col.sameRow)
			itemsInRow++;
		var hdg = $('<th class="form-label"></th>').appendTo(row);
		if (col.preamble) {
			if (!preambleRow) {
				preambleRow = $('<tr class="form-question preamble"></tr>');
				preambleRow.insertBefore(row);
				while (itemsInRow) {
					$('<td class="form-label" colspan="2"></td>').appendTo(preambleRow);
					itemsInRow--;
				}
			}
			$('<td class="form-label" colspan="2"></td>').appendTo(preambleRow).html(col.preamble);
		} else if (preambleRow) {
			$('<td class="form-label" colspan="2"></td>').appendTo(preambleRow);
		}
		var lbl = $('<label for="r0c' + col.name + '"></label>').appendTo(hdg);
		lbl.text(col.heading).attr('title', col.hint);
		if (col.hint)
			lbl.append(' <span class="hint">?</span>');
		col.cell = $('<td class="form-inputs"></td>').appendTo(row).html(col.defaultContent);
		if (col.colspan)
			col.cell.attr('colspan', col.colspan);
		if (col.sClass)
			col.cell.attr('class', col.sClass);
		if (col["@class"])
			row.addClass(col["@class"]);
		columns[col.name] = col;
		col.index = index;
	});
	// Attach event handler to input fields
	$('body').off('change', selector + ' :input');
	$('body').on('change', selector + ' :input', function (/** this: jElement */) {
		$('button#Back').text('Cancel');
		var col = result.fields[$(this).attr('data-col')];
		if (col) {
			var val;
			try {
				//noinspection JSCheckFunctionSignatures
				val = col.inputValue(this, result.data);
			} catch (e) {
				message(col.heading + ':' + e);
				$(this).focus();
				return;
			}
			if (col.change) {
				var nval = col.change(val, result.data, col, this);
				if (nval === false) {
					col.update(col.cell, result.data[col.data], 0, result.data);
					$(this).focus();
					return;
				} else if (nval !== undefined && nval !== null)
					val = nval;
			}
			if ($(selector).triggerHandler('changed.field', [val, result.data, col, this]) !== false) {
				if (this.type == 'file' && $(this).hasClass('autoUpload')) {
					var img = $(this).prev('img');
					var submitHref = defaultUrl('Upload');
					var d = new FormData();
					for (var f = 0; f < this.files.length; f++)
						d.append('file' + (f || ''), this.files[f]);
					d.append('json', JSON.stringify(result.data));
					postFormData(submitHref, d, function (d) {
						if (tableName && d.id)
							window.location = urlParameter('id', d.id);
					});
				} else {
					result.data[col.data] = val;
					_.each(options.columns, function (c) {
						if (c.data == col.data) {
							c.update(c.cell, val, 0, result.data);
						}
					});
				}
			}
		}
	});
	var result = $(selector);

	/**
	 * Redraw form fields
	 */
	function draw() {
		//noinspection JSUnusedLocalSymbols
		_.each(options.columns, function (col, index) {
			var colData = result.data[col.data];
			col.update(col.cell, colData, 0, result.data);
		});
		addJQueryUiControls();
		if (options.readonly)
			result.find('input,select,textarea,button.ui-multiselect').attr('disabled', true);
	}
	var drawn = false;

	/**
	 * Draw form when data arrives
	 * @param d
	 */
	function dataReady(d) {
		result.data = d;
		if (deleteButton && !d[idName])
			deleteButton.remove();
		draw();
		// Only do this bit once
		if (drawn)
			return;
		drawn = true;
		if (submitUrl) {
			if (options.dialog) {
				// Wrap form in a dialog, called by Edit button
				result.wrap('<div id="dialog"></div>');
				result.parent().dialog({
					autoOpen: false,
					modal: true,
					height: Math.min(result.height() + 200, $(window).height() * 0.9),
					width: Math.min(result.width() + 100, $(window).width() - 50),
					buttons: options.readonly ? {
						Ok: {
							id: 'Ok',
							text: 'Ok',
							click: function () {
								$(this).dialog("close");
							}
						}
					} : {
						Ok: {
							id: 'Ok',
							text: 'Ok',
							click: function () {
								if (!options.readonly)
									submitUrl(this);
								$(this).dialog("close");
							}
						},
						Cancel: {
							id: 'Cancel',
							text: 'Cancel',
							click: function () {
								$(this).dialog("close");
							}
						}
					}
				});
				actionButton(options.readonly ? 'View' : 'Edit', 'openDialog')
					.click(function (e) {
						result.parent().dialog('open');
						e.preventDefault();
					});
			} else {
				// Add Buttons
				if (!options.readonly) {
					actionButton(options.submitText || 'Save', 'save')
						.addClass('goback')
						.click(function (e) {
							submitUrl(this);
							e.preventDefault();
						});
					if (options.apply)
						actionButton(options.applyText || 'Apply', 'apply')
							.click(function (e) {
								submitUrl(this);
								e.preventDefault();
							});
					if (options.saveAndNew)
						actionButton((options.submitText || 'Save') + ' and New', 'savenew')
							.addClass('new')
							.click(function (e) {
								submitUrl(this);
								e.preventDefault();
							});
					actionButton('Reset', 'reset')
						.click(function () {
							window.location.reload();
						});
				}
			}
		}
		if (deleteUrl && !options.readonly) {
			deleteButton = actionButton(options.deleteText || 'Delete', 'delete')
				.click(function (e) {
					if (confirm('Are you sure you want to ' + (options.deleteText ? options.deleteText : 'delete this record')))
						deleteUrl(this);
					e.preventDefault();
				});
		}
	}
	result.fields = columns;
	result.settings = options;
	result.dataReady = dataReady;
	result.draw = draw;
	result.submit = submitUrl;
	result.submitCallback = options.submitCallback;
	result.updateSelectOptions = function (col, selectOptions) {
		col.selectOptions = selectOptions;
		col.cell.html(col.draw(result.data[col.data], 0, result.data));
	};


	Forms.push(result);
	if (options.data)
		dataReady(options.data);
	else if (options.ajax) {
		get(options.ajax.url, null, dataReady);
	}
	return result;
}

/**
 * Make a header and detail form
 * @param headerSelector
 * @param detailSelector
 * @param options - has header and detail objects for the 2 parts of the form
 */
function makeHeaderDetailForm(headerSelector, detailSelector, options) {
	var submitUrl = options.submit;
	var tableName = options.header.table;
	if (submitUrl === undefined) {
		submitUrl = defaultUrl('Save');
	}
	if (typeof (submitUrl) == 'string') {
		var submitHref = submitUrl;
		submitUrl = function (button, callback) {
			if (typeof (callback) != 'function')
				callback = null;
			var hdg = null;
			try {
				// Validate everything
				_.each(options.header.columns, function (col) {
					if (col.inputValue) {
						hdg = col.heading;
						col.inputValue(result.header.find('#r0c' + col.name), result.header.data);
					}
				});
				_.each(options.detail.columns, function (col) {
					if (col.inputValue) {
						hdg = col.heading;
						_.each(result.detail.data, function (row, index) {
							col.inputValue(result.detail.find('#r' + index + 'c' + col.name), row);
						});
					}
				});
			} catch (e) {
				message(hdg + ':' + e);
				return;
			}
			if (options.header.validate) {
				var msg = options.header.validate();
				message(msg);
				if (msg) return;
			}
			if (options.validate) {
				msg = options.validate();
				message(msg);
				if (msg) return;
			}
			postJson(submitHref, {
				header: result.header.data,
				detail: result.detail.data
			}, function (d) {
				// Success
				if (callback && callback(d, true))
					return;
				if (result.submitCallback && result.submitCallback(d))
					return;
				if ($(button).hasClass('goback'))
					goback();
				else if ($(button).hasClass('new'))
					window.location = urlParameter('id', 0);
				else if (tableName && d.id)
					window.location = urlParameter('id', d.id);
			}, function (d) {
				// Failure
				if (callback && callback(d, false))
					return;
				if (result.submitCallback)
					result.submitCallback(d);
			});
		};
	}
	if (options.header.submit === undefined)
		options.header.submit = submitUrl;
	options.detail.submit = null;
	if (options.header.readonly === undefined)
		options.header.readonly = options.readonly;
	if (options.detail.readonly === undefined)
		options.detail.readonly = options.header.readonly;
	if (options.header.ajax === undefined || options.detail.ajax === undefined) {
		if (options.data) {
			if (options.header.ajax === undefined)
				options.header.ajax = null;
			if (options.detail.ajax === undefined)
				options.detail.ajax = null;
		} else {
			_setAjaxObject(options, 'Data', 'detail');
			if (options.ajax) {
				get(options.ajax.url, null, dataReady);
			}
		}
	}
	function dataReady(d) {
		result.data = d;
		if (d.header)
			result.header.dataReady(d.header);
		if (d.detail)
			result.detail.dataReady(d.detail);
	}
	var result = {
		header: makeForm(headerSelector, options.header),
		detail: makeListForm(detailSelector, options.detail),
		data: options.data,
		dataReady: dataReady,
		submit: submitUrl,
		submitCallback: options.submitCallback
	};
	result.detail.header = result.header;
	nextPreviousButtons(result.data);
	result.detail.bind('changed.field', function () {
		$('button#Back').text('Cancel');
	});
	if (options.data)
		dataReady(options.data);
	return result;
}

/**
 * Make the detail part of a header detail form
 * @param selector
 * @param options
 * Additional options include:
 * sortable - if true, user can drag and drop wors to sort data
 * addRows - if true, user can add rows
 * emptyRow - if addRows is true, this is the object to add for a new empty row
 * hasData(row) - if addRows is true, this function to call to determine if a row has data in it
 */
function makeListForm(selector, options) {
	var table = $(selector);
	var tableName = myOption('table', options);
	var idName = myOption('id', options, 'id' + tableName);
	var submitUrl = myOption('submit', options);
	var selectUrl = myOption('select', options);
	if (selectUrl === undefined && submitUrl === undefined) {
		submitUrl = defaultUrl('Save');
	}
	if (selectUrl === undefined && typeof (submitUrl) == 'string') {
		var submitHref = submitUrl;
		//noinspection JSUnusedAssignment,JSUnusedLocalSymbols
		submitUrl = function (button, callback) {
			if (typeof (callback) != 'function')
				callback = null;
			try {
				var hdg;
				_.each(options.columns, function (col) {
					if (col.inputValue) {
						hdg = col.heading;
						_.each(table.data, function (row, index) {
							col.inputValue(table.find('#r' + index + 'c' + col.name), row);
						});
					}
				});
			} catch (e) {
				message(e);
				return;
			}
			if (options.validate) {
				var msg = options.validate();
				message(msg);
				if (msg) return;
			}
			postJson(submitHref, table.data, function (d) {
				// Success
				if (callback && callback(d, true))
					return;
				if (result.submitCallback)
					result.submitCallback(d);
			}, function (d) {
				// Failure
				if (callback && callback(d, false))
					return;
				if (result.submitCallback)
					result.submitCallback(d);
			});
		};
	}
	if (typeof (selectUrl) == 'string') {
		var sel = selectUrl;
		selectUrl = function (row, e) {
			goto(urlParameter('id', row[idName], sel), e);
		};
	}
	if (typeof (selectUrl) == 'function') {
		selectClick(selector, function (e) {
			return selectUrl.call(this, table.rowData($(this)), e);
		});
	} else {
		selectClick(selector, null);
	}
	if (options.addRows && (options.deleteRows === true)) {
		options.deleteRows = function (data) {
			if ($(this).index() == table.data.length - 1)
				return false;
		};
	}
	$(selector).addClass('form');
	$(selector).addClass('listform');
	_setAjaxObject(options, 'Listing', '');
	var row = null;
	var columns = {};
	var heading = table.find('thead');
	if (heading.length == 0) heading = $('<thead></thead>').appendTo(table);
	var body = table.find('tbody');
	if (body.length == 0) body = $('<tbody></tbody>').appendTo(table);
	var rowsPerRecord = 0;
	var colCount = 0;
	var c = 0;
	var skip = 0;
	_.each(options.columns, function (col, index) {
		options.columns[index] = col = _setColObject(col, tableName, index);
		if (!row || col.newRow) {
			row = $('<tr></tr>').appendTo(heading);
			row.addClass("r" + ++rowsPerRecord);
			c = 0;
		}
		c++;
		if (skip) {
			skip--;
		} else {
			var cell = $('<th></th>').appendTo(row).text(col.heading).attr('title', col.hint);
			cell.attr('id', 'c-' + col.name);
			if (col.hint)
				cell.append(' <span class="hint">?</span>');
			if (col.colspan) {
				cell.attr('colspan', col.colspan);
				skip = col.colspan - 1;
			}
			if (col.sClass)
				cell.attr('class', col.sClass);
		}
		columns[col.name] = col;
		col.index = index;
		colCount = Math.max(colCount, c);
	});
	if (!options.readonly && rowsPerRecord == 1) {
		if (options.deleteRows)
			$('<th></th>').appendTo(row);
		if (options.sortable)
			$('<th></th>').appendTo(row);
	}
	$('body').off('change', selector + ' :input');
	$('body').on('change', selector + ' :input', function () {
		var col = table.fields[$(this).attr('data-col')];
		if (col) {
			var rowIndex = table.rowIndex(this);
			var val;
			try {
				//noinspection JSCheckFunctionSignatures
				val = col.inputValue(this, table.data[rowIndex]);
			} catch (e) {
				message(col.heading + ':' + e);
				$(this).focus();
				return;
			}
			if (col.change) {
				var nval = col.change(val, table.data[rowIndex], col, this);
				if (nval === false) {
					col.update(col.cell, table.data[rowIndex][col.data], rowIndex, table.data[rowIndex]);
					$(this).focus();
					return;
				} else if (nval !== undefined && nval !== null)
					val = nval;
			}
			if (table.triggerHandler('changed.field', [val, table.data[rowIndex], col, this]) !== false) {
				if (this.type == 'file' && $(this).hasClass('autoUpload')) {
					var img = $(this).prev('img');
					var submitHref = defaultUrl('Upload');
					var d = new FormData();
					for (var f = 0; f < this.files.length; f++)
						d.append('file' + (f || ''), this.files[f]);
					if (table.header)
						d.append('header', JSON.stringify(table.header.data));
					d.append('detail', JSON.stringify(table.data[rowIndex]));
					postFormData(submitHref, d, function (d) {
						if (tableName && d.id)
							window.location = urlParameter('id', d.id);
					});
				} else {
					table.data[rowIndex][col.data] = val;
					row = body.find('tr:eq(' + (rowIndex * rowsPerRecord) + ')');
					var cell = row.find('td:first');
				}
				_.each(options.columns, function (c) {
					if (c.newRow) {
						row = row.next('tr');
						cell = row.find('td:first');
					}
					if (c.data == col.data) {
						c.update(cell, val, rowIndex, table.data[rowIndex]);
					}
					cell = cell.next('td');
				});
			}
			if (options.addRows)
				checkForNewRow();
		}
	});
	/**
	 * Draw an individual row by index
	 * @param rowIndex
	 */
	function drawRow(rowIndex) {
		var row = null;
		var cell = null;
		var rowData = table.data[rowIndex];
		var rowno = 1;
		var isNewRow;
		function newRow(r) {
			isNewRow = r.length == 0;
			if (isNewRow)
				r = $('<tr></tr>').appendTo(body);
			row = r;
			if (rowData["@class"])
				row.addClass(rowData["@class"]);
			row.addClass("r" + rowno++);
			cell = row.find('td:first');
		}
		newRow(body.find('tr:eq(' + (rowIndex * rowsPerRecord) + ')'));
		_.each(options.columns, function (col) {
			if (col.newRow)
				newRow(row.next('tr'));
			if (cell.length == 0) {
				cell = $('<td></td>').appendTo(row);
				cell.attr('aria-labelled-by', 'c-' + col.name);
				if (col.sClass)
					cell.attr('class', col.sClass);
			}
			var data = rowData[col.data];
			col.update(cell, data, rowIndex, rowData);
			cell = cell.next('td');
		});
		if (isNewRow && !options.readonly && rowsPerRecord == 1) {
			if (options.deleteRows) {
				if (cell.length != 0)
					cell.remove();
				cell = $('<td class="deleteButton"></td>').appendTo(row);
				$('<button class="deleteButton"><img src="/images/close.png" /></button>').appendTo(cell).click(function () {
					var thisrow = $(this).closest('tr');
					var index = thisrow.index();
					var callback;
					if (typeof (options.deleteRows) == "function")
						callback = options.deleteRows.call(thisrow, table.data[index]);
					if (callback != false) {
						unsavedInput = true;
						$('button#Back').text('Cancel');
						thisrow.remove();
						table.data.splice(index, 1);
						if (typeof (callback) == 'function')
							callback();
					}
				});
			}
			if (options.sortable)
				$('<td class="draghandle" data-row="' + rowIndex + '"><div class="ui-icon-arrowthick-2-n-s"/></td>').appendTo(row);
		}
	}

	function hasData(row) {
		if (options.hasData && typeof (options.hasData) == 'function')
			return options.hasData(row);
		emptyRow = options.emptyRow === undefined ? {} : options.emptyRow;
		for (var key in row)
			if (!key.match(/^@/) && row[key] && row[key] != emptyRow[key])
				return true;

	}
	function checkForNewRow() {
		var lastRow = table.data[table.data.length - 1];
		if (lastRow == null || hasData(lastRow)) {
			if (lastRow)
				delete lastRow['@class'];
			table.find('tbody tr.noDeleteButton').removeClass('noDeleteButton');
			var newRow = options.emptyRow === undefined ? {} : _.clone(options.emptyRow);
			newRow['@class'] = 'noDeleteButton';
			table.addRow(newRow);
			return true;
		}
	}
	var dragFrom, dragTo;
	/**
	 * Draw the whole form
	 */
	function draw() {
		for (var row = 0; row < table.data.length; row++) {
			drawRow(row);
		}
		addJQueryUiControls();
		if (options.readonly)
			table.find('input,select,textarea,button.ui-multiselect').attr('disabled', true);
		if (options.sortable && !options.readonly && rowsPerRecord == 1) {
			table.find('tbody').sortable({
				items: "> tr:not(.noDeleteButton)",
				appendTo: "parent",
				// helper: "clone",
				axis: 'y',
				containment: table,
				handle: 'td.draghandle',
				start: function (event, ui) {
					dragFrom = ui.item.index();
					console.log('Start: from=' + dragFrom);
				},
				update: function (event, ui) {
					var data = [];
					table.find('tbody tr td.draghandle').each(function (index) {
						var r = parseInt($(this).attr('data-row'));
						if (index != r) {
							unsavedInput = true;
							$('button#Back').text('Cancel');
						}
						data.push(table.data[r]);
					});
					if (data.length) {
						dragTo = ui.item.index();
						console.log('Update: from=' + dragFrom + ' to=' + dragTo);
						table.dataReady(data);
						if (dragFrom != dragTo)
							table.triggerHandler('dragged.row', [dragFrom, dragTo]);
					}
				}
			}).disableSelection();
		}
	}
	var drawn = false;
	function dataReady(d) {
		table.data = d;
		if (options.addRows && !options.readonly && rowsPerRecord == 1) {
			checkForNewRow();
		}
		body.find('tr').remove();
		draw();
		if (!drawn && submitUrl) {
			drawn = true;
			if (!options.readonly) {
				actionButton(options.submitText || 'Save', 'save')
					.addClass('goback')
					.click(function (e) {
						submitUrl(this);
						e.preventDefault();
					});
				if (options.apply)
					actionButton(options.applyText || 'Apply', 'apply')
						.click(function (e) {
							submitUrl(this);
							e.preventDefault();
						});
				actionButton('Reset', 'reset')
					.click(function () {
						window.location.reload();
					});
			}
		}
	}

	/**
	 * Redraw
	 */
	function refresh() {
		if (options.data)
			dataReady(options.data);
		else if (options.ajax) {
			body.html('<tr><td colspan="' + colCount + '" style="text-align: center;">Loading...</td></tr>');
			get(options.ajax.url, null, dataReady);
		}
	}
	table.fields = columns;
	table.settings = options;
	table.dataReady = dataReady;
	table.draw = draw;
	table.refresh = refresh;
	table.submit = submitUrl;
	table.submitCallback = options.submitCallback;
	/**
	 * Return the row index of item r
	 * @param r
	 * @returns {number}
	 */
	table.rowIndex = function (r) {
		r = $(r);
		if (r.attr('tagName') != 'TR')
			r = r.closest('tr');
		return Math.floor(r.index() / rowsPerRecord);
	};
	/**
	 * Return the row data for item r
	 * @param r
	 * @returns {*}
	 */
	table.rowData = function (r) {
		return table.data[table.rowIndex(r)];
	};
	/**
	 * Draw the row
	 * @param {number|jElement} r rowIndex or item in a row
	 */
	table.drawRow = function (r) {
		if (typeof (r) != 'number')
			r = table.rowIndex(r);
		drawRow(r);
	};
	/**
	 * Add a new row
	 * @param row The data to add
	 */
	table.addRow = function (row) {
		table.data.push(row);
		drawRow(table.data.length - 1);
		addJQueryUiControls();
	};
	/**
	 * The cell for a data item
	 * @param rowIndex
	 * @param col The col object
	 * @returns {*|{}}
	 */
	table.cellFor = function (rowIndex, col) {
		var row = body.find('tr:eq(' + (rowIndex * rowsPerRecord) + ')');
		var cell = row.find('td:first');
		for (var c = 0; c < options.columns.length; c++) {
			if (options.columns[c].newRow) {
				row = row.next();
				cell = row.find('td:first');
			}
			if (options.columns[c] == col)
				return cell;
			cell = cell.next();
		}
	};
	table.updateSelectOptions = function (col, selectOptions) {
		col.selectOptions = selectOptions;
		_.each(table.data, function (rowData, index) {
			var cell = table.cellFor(index, col);
			cell.html(col.draw(rowData[col.data], index, rowData));
		});
	};
	refresh();
	Forms.push(table);
	return table;
}

/**
 * Make a form to edit a single record and post it back
 * Column options are:
 * {string} [prefix]dataItemName[/heading] (prefix:#=decimal, /=date, @=email)
 * or
 * {*} [type] Type.* - sets defaults for column options
 * {string} data item name
 * {string} [heading]
 * @param {string} selector
 * @param options
 * @param {string} [options.table] Name of SQL table
 * @param {string} [options.idName] Name of id field in table (id<table>)
 * @param {string|function} [options.select] Url to go to or function to call when a row is clicked
 * @param {string|*} [options.ajax] Ajax settings, or string url (current url + 'Listing')
 * @param {boolean} [options.dialog] Show form as a dialog when Edit button is pushed
 * @param {string} [options.submitText} Text to use for Save buttons (default "Save")
 * @param {boolean} [options.readonly} No save (default false)
 * @param {*} [options.data] Existing data to display
 * @param {Array} options.columns
 * @param {function} [options.validate] Callback to validate data
 * @returns {*}
 */
function makeDumbForm(selector, options) {
	var tableName = myOption('table', options);
	var submitUrl = myOption('submit', options);
	var canDelete = myOption('canDelete', options);
	var deleteButton;
	if (submitUrl === undefined) {
		submitUrl = defaultUrl('Save');
	}
	if (submitUrl) {
		$(selector).closest('form').attr("action", submitUrl);
	}
	var deleteUrl = canDelete ? myOption('delete', options) : null;
	if (deleteUrl === undefined) {
		deleteUrl = defaultUrl('Delete');
	}
	if (typeof (deleteUrl) == 'string') {
		var deleteHref = deleteUrl;
		//noinspection JSUnusedLocalSymbols
		deleteUrl = function (button) {
			postJson(deleteHref, result.data, goback);
		};
	}
	$(selector).addClass('form');
	_setAjaxObject(options, 'Data', '');
	var row;
	var columns = {};
	_.each(options.columns, function (col, index) {
		options.columns[index] = col = _setColObject(col, tableName, index);
		if (!row || !col.sameRow)
			row = $('<tr class="form-question"></tr>').appendTo($(selector));
		var hdg = $('<th class="form-label"></th>').appendTo(row);
		var lbl = $('<label for="r0c' + col.name + '"></label>').appendTo(hdg);
		lbl.text(col.heading).attr('title', col.hint);
		if (col.hint)
			lbl.append(' <span class="hint">?</span>');
		col.cell = $('<td class="form-inputs"></td>').appendTo(row).html(col.defaultContent);
		if (col.colspan)
			col.cell.attr('colspan', col.colspan);
		columns[col.name] = col;
		col.index = index;
	});
	// Attach event handler to input fields
	$('body').off('change', selector + ' :input');
	$('body').on('change', selector + ' :input', function (/** this: jElement */) {
		$('button#Back').text('Cancel');
		var col = result.fields[$(this).attr('data-col')];
		if (col) {
			var val;
			try {
				//noinspection JSCheckFunctionSignatures
				val = col.inputValue(this, result.data);
			} catch (e) {
				message(col.heading + ':' + e);
				$(this).focus();
				return;
			}
			if (col.change) {
				var nval = col.change(val, result.data, col, this);
				if (nval === false) {
					col.update(col.cell, result.data[col.data], 0, result.data);
					$(this).focus();
					return;
				} else if (nval !== undefined && nval !== null)
					val = nval;
			}
			if ($(selector).triggerHandler('changed.field', [val, result.data, col, this]) !== false) {
				if (this.type == 'file' && $(this).hasClass('autoUpload')) {
					var img = $(this).prev('img');
					var submitHref = defaultUrl('Upload');
					var d = new FormData();
					for (var f = 0; f < this.files.length; f++)
						d.append('file' + (f || ''), this.files[f]);
					d.append('json', JSON.stringify(result.data));
					postFormData(submitHref, d, function (d) {
						if (tableName && d.id)
							window.location = urlParameter('id', d.id);
					});
				} else {
					result.data[col.data] = val;
					_.each(options.columns, function (c) {
						if (c.data == col.data) {
							c.update(c.cell, val, 0, result.data);
						}
					});
				}
			}
		}
	});
	var result = $(selector);

	/**
	 * Redraw form fields
	 */
	function draw() {
		//noinspection JSUnusedLocalSymbols
		_.each(options.columns, function (col, index) {
			var colData = result.data[col.data];
			col.update(col.cell, colData, 0, result.data);
		});
		addJQueryUiControls();
		if (options.readonly)
			result.find('input,select,textarea').attr('disabled', true);
	}
	var drawn = false;

	/**
	 * Draw form when data arrives
	 * @param d
	 */
	function dataReady(d) {
		result.data = d;
		draw();
		$(selector).find(':input').each(function () { this.name = $(this).attr('data-col'); });
		// Only do this bit once
		if (drawn)
			return;
		drawn = true;
		if (submitUrl) {
			// Add Buttons
			if (!options.readonly) {
				actionButton(options.submitText || 'Save', 'save')
					.click(function (e) {
						unsavedInput = false;
						$(selector).closest('form').submit();
						e.preventDefault();
					});
				actionButton('Reset', 'reset')
					.click(function () {
						window.location.reload();
					});
			}
		}
	}
	if (deleteUrl && !options.readonly) {
		deleteButton = actionButton(options.deleteText || 'Delete', 'delete')
			.click(function (e) {
				if (confirm('Are you sure you want to ' + (options.deleteText ? options.deleteText : 'delete this record')))
					deleteUrl(this);
				e.preventDefault();
			});
	}
	result.fields = columns;
	result.settings = options;
	result.dataReady = dataReady;
	result.draw = draw;
	result.submit = submitUrl;
	result.submitCallback = options.submitCallback;
	Forms.push(result);
	if (options.data)
		dataReady(options.data);
	else if (options.ajax) {
		get(options.ajax.url, null, dataReady);
	}
	return result;
}

/**
 * The data for download. Returns array of arrays of fields.
 */
function downloadData(columns, record) {
	var data = [];
	var dataRow = [];
	var skip;
	_.each(columns, function (col, index) {
		if (col.newRow) {
			data.push(dataRow);
			dataRow = [];
		}
		if (skip) {
			skip--;
			dataRow.push('');
		} else {
			dataRow.push(col.heading);
			if (col.colspan)
				skip = col.colspan - 1;
		}
	});
	data.push(dataRow);
	function buildRow(row, rowdata) {
		dataRow = [];
		_.each(columns, function (col, index) {
			if (col.newRow) {
				data.push(dataRow);
				dataRow = [];
			}
			dataRow.push(col.download(rowdata[col.name], row, rowdata));
		});
		data.push(dataRow);
	}
	if (record[1]) {
		for (var row = 0; row < record.length; row++) {
			buildRow(row, record[row]);
		}
	} else {
		buildRow(0, record);
	}
	return data;
}

/**
 * Extract a named option from opts, remove it, and return it (or defaultValue if not present)
 * @param {string} name
 * @param {*} opts
 * @param {string} [defaultValue]
 * @returns {*}
 */
function myOption(name, opts, defaultValue) {
	var result = opts[name];
	if (result === undefined) {
		result = defaultValue;
	} else {
		if (typeof (result) != 'function') result = _.clone(result);
		delete opts[name];
	}
	return result;
}

/**
 * Add next and previous buttons to a document display
 * @param {number} record.next id of next record
 * @param {number} record.previous id of previous record
 */
function nextPreviousButtons(record) {
	if (record && record.previous != null) {
		actionButton('Previous', 'previous')
			.click(function () {
				window.location = urlParameter('id', record.previous);
			});
	}
	if (record && record.next != null) {
		actionButton('Next', 'next')
			.click(function () {
				window.location = urlParameter('id', record.next);
			});
	}
}

/**
 * Post data to url
 * @param {string} url
 * @param data
 * @param {function} [success]
 * @param {function} [failure]
 */
function postJson(url, data, success, failure) {
	if (typeof (data) == 'function') {
		failure = success;
		success = data;
		data = {};
	}
	if (data == null)
		data = {};
	postData(url, { json: JSON.stringify(data) }, false, success, failure);
}

/**
 * Post form data containing uploaded file
 * @param {string} url
 * @param data
 * @param {function} [success]
 * @param {function} [failure]
 */
function postFormData(url, data, success, failure) {
	postData(url, data, true, success, failure, 60000);
}

/**
 * Post data
 * @param {string} url
 * @param data
 * @param {boolean} asForm true to post as multiplart/form-data (uploaded file)
 * @param {function} [success]
 * @param {function} [failure]
 * @param {number} [timeout] in msec
 */
function postData(url, data, asForm, success, failure, timeout) {
	if (typeof (failure) != 'function') {
		timeout = failure;
		failure = undefined;
	}
	message(timeout > 10000 ? 'Please wait, uploading data...' : 'Please wait...');
	var ajax = {
		url: url,
		type: 'post',
		data: data,
		timeout: timeout || 10000,
		xhrFields: {
			withCredentials: true
		}
	};
	if (asForm) {
		ajax.enctype = 'multipart/form-data';
		ajax.processData = false;
		ajax.contentType = false;
	}
	$.ajax(ajax)
		.done(
			/**
			 * @param {string} [result.error] Error message
			 * @param {string} [result.message] Info message
			 * @param {string} [result.confirm] Confirmation question
			 * @param {string} [result.redirect] Where to go now
			 */
			function (result) {
				if (result.error) {
					message(result.error);
					if (failure)
						failure(result);
				} else {
					message(result.message);
					if (result.confirm) {
						// C# code wants a confirmation
						if (confirm(result.confirm)) {
							url += /\?/.test(url) ? '&' : '?';
							url += 'confirm';
							postData(url, data, asForm, success, timeout);
						}
						return;
					}
					unsavedInput = false;
					if (success && !result.redirect) {
						success(result);
						return;
					}
				}
				if (result.redirect)
					window.location = result.redirect;
			})
		.fail(function (jqXHR, textStatus, errorThrown) {
			var txt = textStatus == errorThrown ? textStatus : textStatus + ' ' + errorThrown;
			message(txt);
			if (failure)
				failure({ error: txt });
		});
}

/**
 * Round a number to 2 decimal places
 * @param {number} v
 * @returns {number}
 */
function round(v) {
	return Math.round(100 * v) / 100;
}

/**
 * Add selected class to just this row
 */
function selectOn() {
	$(this).siblings('tr').removeClass('selected');
	$(this).addClass('selected');
}

/**
 * Remove selected class from this row
 */
function selectOff() {
	$(this).removeClass('selected');
}

/**
 * Add mouse handlers for table rows
 * @param {string} selector table
 * @param {function} selectFunction (returns false if row can't be selected)
 */
function selectClick(selector, selectFunction) {
	$('body').off('click', selector + ' tbody td:not(:has(input))');
	if (!touchScreen) {
		$('body').off('mouseenter', selector + ' tbody tr')
			.off('mouseleave', selector + ' tbody tr');
	}
	if (!selectFunction)
		return;
	var table = $(selector);
	table.addClass('noselect');
	table.find('tbody').css('cursor', 'pointer');
	$('body').on('click', selector + ' tbody td:not(:has(input))', function (e) {
		if (e.target.tagName == 'A')
			return;
		if (table.hasClass('collapsed') && $(e.target).hasClass('dtr-control') && e.clientX < 35)
			return;		// Responsive data-table click on +/- sign
		var row = $(this).closest('tr');
		// On touch screens, tap something once to select, twice to open it
		// On ordinary screens, click once to open (mouseover selects)
		var select = !touchScreen || !doubleClickToSelect || row.hasClass('selected');
		selectOn.call(row);
		if (select && selectFunction.call(this, e) == false)
			selectOff.call(row);
		e.preventDefault();
		e.stopPropagation();
		return false;
	});
	if (!touchScreen) {
		// Mouse over highlights row
		$('body').on('mouseenter', selector + ' tbody tr', selectOn)
			.on('mouseleave', selector + ' tbody tr', selectOff);
	}
}

/**
 * Add defaultSuffix to the current url
 * @param {string} defaultSuffix
 * @returns {string}
 */
function defaultUrl(defaultSuffix) {
	var url = window.location.pathname.replace(/\.html$/, '');
	if (url == '/')
		url = '/home/default';
	else if (url.substring(1).indexOf('/') < 0)
		url += '/default';
	return url + defaultSuffix + ".html" + window.location.search;
}

/**
 * Change (or add or delete) the value of a named parameter in a url
 * @param {string} name of parameter
 * @param {string|number} [value] new value (null or missing to delete)
 * @param {string} [url] If missing, use current url with any message removed
 * @returns {string}
 */
function urlParameter(name, value, url) {
	if (url === undefined) { //noinspection JSCheckFunctionSignatures
		url = urlParameter('message', null, window.location.href);
	}
	var regex = new RegExp('([\?&])' + name + '(=[^\?&]*)?');
	if (value === null || value === undefined) {
		var m = regex.exec(url);
		if (m)
			url = url.replace(regex, m[1] == '?' ? '?' : '').replace('?&', '?');
	} else if (regex.test(url))
		url = url.replace(regex, '$1' + name + '=' + value);
	else
		url += (url.indexOf('?') < 0 ? '?' : '&') + name + '=' + value;
	return url;
}

/**
 * If options.data is not present, set options.ajax to retrieve the data
 * @param options
 * @param {string} defaultSuffix to add to current url if options.ajax is undefined
 * @param {string} defaultDataSrc Element in returned data to use for form data (or '')
 * @private
 */
function _setAjaxObject(options, defaultSuffix, defaultDataSrc) {
	if (typeof (options.ajax) == 'string') {
		options.ajax = {
			url: options.ajax
		};
	} else if (options.ajax === undefined && !options.data) {
		options.ajax = {
			url: defaultUrl(defaultSuffix)
		};
	}
	if (options.ajax && typeof (options.ajax) == 'object' && options.ajax.dataSrc === undefined) {
		options.ajax.dataSrc = defaultDataSrc;
	}
}

//noinspection JSUnusedLocalSymbols
/**
 * Default inputValue function for a column that doesn't have one
 * @param {jElement} field
 * @param {*} row
 * @returns {string}
 */
function getValueFromField(field, row) {
	return $(field).val();
}

/**
 * Standard update function for a column
 * @param {string} selector to find the input field in the cell
 * @param {jElement} cell
 * @param data
 * @param {number} rowno
 * @param col
 * @param row
 */
function colUpdate(selector, cell, data, rowno, col, row) {
	var i = cell.find(selector);
	if (i.length && i.attr('id')) {
		// Field exists
		i.val(data);
	} else {
		// No field yet - draw one
		cell.html(col.draw(data, rowno, row));
	}
}

//noinspection JSUnusedLocalSymbols
/**
 * Default render function for a column that doesn't have one
 * @param data
 * @param {string} type
 * @param row
 * @param {*} meta
 * @param {number} meta.row
 * @param {number} meta.col
 * @param {Array} meta.settings.oInit.columns
 * @returns {string}
 */
function colRender(data, type, row, meta) {
	switch (type) {
		case 'display':
		case 'filter':
			var col = meta.settings.oInit.columns[meta.col];
			return col.draw(data, meta.row, row);
		default:
			return data;
	}
}

//noinspection JSUnusedLocalSymbols
/**
 * Default render function for a number
 * @param data
 * @param {string} type
 * @param row
 * @param {*} meta
 * @param {number} meta.row
 * @param {number} meta.col
 * @param {Array} meta.settings.oInit.columns
 * @returns {string}
 */
function numberRender(data, type, row, meta) {
	switch (type) {
		case 'display':
			return colRender(data, type, row, meta);
		case 'filter':
			return formatNumber(data);
		default:
			return data;
	}
}

//noinspection JSUnusedLocalSymbols,JSUnusedLocalSymbols
/**
 * Default draw function for a column that doesn't have one
 * @param data
 * @param {number} rowno
 * @param row
 * @returns {string}
 */
function colDraw(data, rowno, row) {
	return new SafeHtml().text(data).html();
}

/**
 * Set up column option defaults
 * @param col
 * @param {string} tableName
 * @param {int} index
 * @returns {*} col
 * @private
 */
function _setColObject(col, tableName, index) {
	var type;
	if (typeof (col) == 'string') {
		// Shorthand - [#/@]name[/heading]
		switch (col[0]) {
			case '#':
				type = 'decimal';
				col = col.substring(1);
				break;
			case '/':
				type = 'date';
				col = col.substring(1);
				break;
			case '@':
				type = 'email';
				col = col.substring(1);
				break;
		}
		var split = col.split('/');
		col = { data: split[0] };
		if (split.length > 1) col.heading = split[1];
		col.type = type;
	} else {
		type = col.type;
	}
	if (type) _.defaults(col, Type[type]);
	if (col.attributes == null)
		col.attributes = '';
	if (!col.name) col.name = col.data.toString();
	if (col.heading === undefined) {
		var title = col.name;
		// Remove table name from front
		if (tableName && title.indexOf(tableName) == 0 && title != tableName)
			title = title.substring(tableName.length);
		// Split "CamelCase" name into "Camel Case", and remove Id from end
		title = title.replace(/Id$/, '').replace(/([A-Z])(?=[a-z0-9])/g, " $1").trim();
		col.heading = title;
	}
	if (typeof (col.defaultContent) == "function") {
		col.defaultContent = col.defaultContent(index, col);
	}
	if (col.inputValue === undefined)
		col.inputValue = getValueFromField;
	if (col.download === undefined)
		col.download = colDraw;
	if (col.draw === undefined)
		col.draw = col.download;
	if (col.render === undefined)
		col.render = colRender;		// Render function for dataTable
	if (!col.update)
		col.update = function (cell, data, rowno, row) {
			cell.html(this.draw(data, rowno, row));
		};
	return col;
}

/**
 * Get a url
 * @param {string} url
 * @param data
 * @param {function} success
 * @param {function} failure
 */
function get(url, data, success, failure) {
	$.ajax({
		url: url,
		type: 'get',
		data: data,
		timeout: 10000,
		xhrFields: {
			withCredentials: true
		}
	})
		.done(success)
		.fail(failure || function (jqXHR, textStatus, errorThrown) {
			message(textStatus == errorThrown ? textStatus : textStatus + ' ' + errorThrown);
		});
}

/**
 * Populate a select with options
 * @param {jElement} select
 * @param {Array} data The options array
 * @param {string} val current value of data item
 * @param {*} [col]
 * @param {boolean} [col.date] True if value to be formatted as date
 * @param {string} [col.emptyValue] Value to use if val is null or missing
 * @param {string} [col.emptyOption] Text to use if val does not match any option
 */
function addOptionsToSelect(select, data, val, col) {
	var found;
	var category;
	var optgroup = select;
	var multi = select.prop('multiple');
	var array = multi && Array.isArray(val);
	var date = col && col.date;
	if (val == null)
		val = col && col.emptyValue != null ? col.emptyValue : '';
	_.each(data,
		/**
		 * Populate a select with options
		 * @param {string} opt.value the text to display
		 * @param {string?} [opt.id] the value to return (value if not supplied)
		 * @param {boolean} [opt.hide]
		 * @param {string} [opt.category] For categorised options
		 * @param {string}[opt.class] css class
		 */
		function (opt) {
			var id = opt.id;
			if (id === undefined)
				id = opt.value;
			if (opt.hide && id != val)
				return;
			var option = $('<option></option>');
			option.attr('value', id);
			option.text(date ? formatDate(opt.value) : opt.value);
			if (id == val || (array && val.indexOf(id))) {
				option.attr('selected', 'selected');
				found = true;
			}
			if (opt.category && opt.category != category) {
				category = opt.category;
				optgroup = $('<optgroup></optgroup>').attr('label', opt.category).appendTo(select);
			}
			if (opt['class'])
				option.addClass(opt['class']);
			option.appendTo(opt.category ? optgroup : select);
		});
	if (!found && !multi) {
		var option = $('<option></option>');
		if (col && col.emptyOption)
			option.text(col.emptyOption);
		option.attr('value', val);
		option.prependTo(select);
	}
	select.val(val);
}

/**
 * Make an indexed hash from an array
 * @param {Array} array
 * @param {string} [key] (default 'id')
 * @returns {{}}
 */
function hashFromArray(array, key) {
	var hash = {};
	if (key == null) key = 'id';
	_.each(array, function (value) {
		hash[value[key]] = value;
	});
	return hash;
}

/**
 * Stores the current url and datatable "Show All" parameters
 * @returns {string}
 */
function getTableUrl() {
	var dt = [];
	$('button[data-nz]').each(function () {
		dt.push($(this).attr('data-nz') == 'true' ? 1 : 0);
	});
	return urlParameter('dt', dt.toString());
}

/**
 * Build a url which stores the current url and datatable "Show All" parameters as "from"
 * @param {string} url base url to go to
 * @returns {string}
 */
function getGoto(url) {
	var current = getTableUrl();
	return urlParameter('from', encodeURIComponent(current), url);
}

/**
 * Go to url, storing the current url and datatable "Show All" parameters as "from"
 * @param url
 */
function goto(url, e) {
	var href = getGoto(url);
	if (e && e.ctrlKey)
		window.open(href, "_blank");
	else
		window.location = href;
}

/**
 * Go back to previous url
 */
function goback() {
	var from = getParameter('from');
	if (!from) {
		from = window.location.pathname;
		var pos = from.substring(1).indexOf('/');
		if (pos >= 0)
			from = from.substring(0, pos + 1);
	}
	window.location = from;
}

/**
 * Get the value of a url parameter
 * @param name
 * @returns {string}
 */
function getParameter(name) {
	var re = new RegExp('[&?]' + name + '=([^&#]*)');
	var m = re.exec(window.location.search);
	return m == null || m.length == 0 ? null : decodeURIComponent(m[1]);
}

//noinspection JSUnusedGlobalSymbols
/**
 * Return true if current url has named parameter
 * @param name
 * @returns {boolean}
 */
function hasParameter(name) {
	var re = new RegExp('[&?]' + name + '(=|&|$)');
	var m = re.exec(window.location.search);
	return m != null && m.length != 0;
}

function textQuote(text) {
	return text.replace(/[\r]/g, '\r').replace(/[\n]/g, '\n').replace(/[\t]/g, '\t');
}

function csvQuote(text) {
	return text.match(/[",\r\n]/) ?
		'"' + text.replace(/"/g, '""') + '"' :
		text;
}

function download(button, data) {
	var menu = $('ul.menu');
	if (menu.length) {
		menu.remove();
		return;
	}
	menu = $('<ul class="menu"><li id="txt">Tab-delimited</li><li id="csv">CSV</li><li id="clip">Copy to clipboard</li></ul>');
	var p = $(button).offset();
	menu.css('left', p.left + 'px').css('top', (p.top + $(button).height()) + 'px');
	menu.appendTo(body);
	var mnu = menu.menu({
		select: function (e, ui) {
			var text = '';
			var tab = ui.item[0].id == 'csv' ? ',' : "\t";
			var quote = ui.item[0].id == 'csv' ? csvQuote : textQuote;
			_.each(data, function (row) {
				var rowText = '';
				_.each(row, function (datum, index) {
					if (index)
						rowText += tab;
					rowText += datum === null || datum === undefined ? '' : quote(datum + '');
				});
				text += rowText + "\r\n";
			});
			switch (ui.item[0].id) {
				case 'txt':
					downloadFile(document.title + '.txt', text);
					break;
				case 'csv':
					downloadFile(document.title + '.csv', text);
					break;
				case 'clip':
					navigator.clipboard.writeText(text);
					break;
			}
			menu.remove();
		}
	});
}

function downloadFile(filename, text) {
	var element = document.createElement('a');
	element.setAttribute('href', 'data:text/plain;charset=windows-1252,' + encodeURIComponent(text));
	element.setAttribute('download', filename);
	element.style.display = 'none';
	document.body.appendChild(element);
	element.click();
	document.body.removeChild(element);
}

/**
 * Add years to a date
 * @param {number} y
 */
Date.prototype.addYears = function (y) {
	this.setYear(this.getYear() + 1900 + y);
};

/**
 * Add months to a date
 * @param {number} m
 */
Date.prototype.addMonths = function (m) {
	var month = (this.getMonth() + m) % 12;
	this.setMonth(this.getMonth() + m);
	while (this.getMonth() > month)
		this.addDays(-1);
};

/**
 * Add days to a date
 * @param {number} d
 */
Date.prototype.addDays = function (d) {
	this.setDate(this.getDate() + d);
};

/**
 * Convert this date to yyyy-mm-dd format
 * @returns {string}
 */
Date.prototype.toYMD = function () {
	var y = this.getYear() + 1900;
	var m = (this.getMonth() + 101).toString().substring(1);
	var d = (this.getDate() + 100).toString().substring(1);
	return y + "-" + m + "-" + d;
};