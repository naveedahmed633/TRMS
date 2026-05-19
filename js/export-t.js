(function (window, document) {
    "use strict";

    var PDF_BUTTON_IDS = [
        "generate-monthly-time-sheet",
        "generate-monthly-punched-photo",
        "generate-captured-photo-sheet",
        "generate-loan-report",
        "generate-payroll-report",
        "department-attendance-pdf-btn",
        "department-report-pdf-btn",
        "miss-punch-report-pdf-btn",
        "manual-punch-report-pdf-btn"
    ];

    function hasJsPdf() {
        return !!((window.jspdf && window.jspdf.jsPDF) || window.jsPDF);
    }

    function getJsPdfCtor() {
        return (window.jspdf && window.jspdf.jsPDF) || window.jsPDF;
    }

    function closestVisibleTable() {
        var selectors = [
            "#datatable-buttons",
            "#datatable",
            ".dataTable",
            "table.table",
            "table"
        ];

        for (var i = 0; i < selectors.length; i++) {
            var nodes = document.querySelectorAll(selectors[i]);
            for (var j = 0; j < nodes.length; j++) {
                var el = nodes[j];
                if (el && el.offsetParent !== null && el.querySelectorAll("tr").length > 1) {
                    return el;
                }
            }
        }

        return null;
    }

    function getReportTitle() {
        var headingSelectors = [
            ".panel-body h3",
            ".card-box h3",
            ".page-title-box h3",
            ".page-title"
        ];

        for (var i = 0; i < headingSelectors.length; i++) {
            var heading = document.querySelector(headingSelectors[i]);
            if (!heading) continue;
            var raw = (heading.textContent || "").replace(/\s+/g, " ").trim();
            if (raw) return raw;
        }

        return (document.title || "Report").trim();
    }

    function buildFileName(reportTitle, isArabic) {
        var title = (reportTitle || "").trim();
        if (!title) {
            title = isArabic ? "تقرير" : "report";
        }

        // Keep Arabic/Unicode letters; remove only invalid filename characters.
        title = title.replace(/[\\/:*?"<>|]+/g, "").replace(/\s+/g, " ").trim();
        if (!title) {
            title = isArabic ? "تقرير" : "report";
        }

        var fileDate = new Date().toISOString().slice(0, 10);
        return title + "-" + fileDate + ".pdf";
    }

    function isArabicLanguage() {
        var ddl = document.querySelector("#ddl_language");
        if (ddl && (ddl.value || "").toLowerCase() === "ar") {
            return true;
        }

        var hiddenLang = document.querySelector("input[name='lang'], input[name='GV_Langauge']");
        if (hiddenLang && (hiddenLang.value || "").toLowerCase() === "ar") {
            return true;
        }

        var htmlLang = (document.documentElement.getAttribute("lang") || "").toLowerCase();
        return htmlLang.indexOf("ar") === 0;
    }

    var PDF_UNICODE_FONT = "Amiri-Regular";
    var ARABIC_SCRIPT_RE = /[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF]/;
    var MOJIBAKE_RE = /[þÿÃÂØÙÚÛÜÝÞßà-ï]{2,}/;

    function hasArabicScript(value) {
        if (value == null) return false;
        return ARABIC_SCRIPT_RE.test(String(value));
    }

    function hasLikelyMojibake(value) {
        if (value == null) return false;
        return MOJIBAKE_RE.test(String(value));
    }

    function bytesFromLatin1String(text) {
        var bytes = new Uint8Array(text.length);
        for (var i = 0; i < text.length; i++) {
            bytes[i] = text.charCodeAt(i) & 0xff;
        }
        return bytes;
    }

    function tryDecodeBytes(bytes, encoding) {
        try {
            if (typeof TextDecoder === "undefined") {
                return "";
            }
            return new TextDecoder(encoding).decode(bytes);
        } catch (e) {
            return "";
        }
    }

    function repairUtf8Mojibake(value) {
        if (value == null) return "";
        var text = String(value);
        if (!text || hasArabicScript(text)) return text;

        if (!hasLikelyMojibake(text) && !/[\u0080-\u00ff]/.test(text)) {
            return text;
        }

        var bytes = bytesFromLatin1String(text);

        var fromCp1256 = tryDecodeBytes(bytes, "windows-1256");
        if (fromCp1256 && hasArabicScript(fromCp1256)) {
            return fromCp1256;
        }

        var fromUtf8 = tryDecodeBytes(bytes, "utf-8");
        if (fromUtf8 && hasArabicScript(fromUtf8)) {
            return fromUtf8;
        }

        try {
            var repaired = decodeURIComponent(escape(text));
            if (repaired && hasArabicScript(repaired)) {
                return repaired;
            }
        } catch (e1) {
            // fall through
        }

        return text;
    }

    function normalizePdfText(value) {
        return repairUtf8Mojibake(cleanTextValue(value));
    }

    function tableHasArabicContent(table) {
        if (!table) return false;
        var cells = table.querySelectorAll("th, td");
        for (var i = 0; i < cells.length; i++) {
            var text = normalizePdfText(cells[i].textContent || "");
            if (hasArabicScript(text) || hasLikelyMojibake(text)) return true;
        }
        return false;
    }

    function shouldUseUnicodeFont(isArabic, table, extraTexts) {
        if (isArabic) return true;
        if (tableHasArabicContent(table)) return true;
        if (extraTexts) {
            for (var i = 0; i < extraTexts.length; i++) {
                var normalized = normalizePdfText(extraTexts[i]);
                if (hasArabicScript(normalized) || hasLikelyMojibake(normalized)) return true;
            }
        }
        return false;
    }

    function registerPdfUnicodeFonts(doc) {
        if (!doc) return false;

        try {
            if (doc.getFontList && doc.getFontList()[PDF_UNICODE_FONT]) {
                return true;
            }
        } catch (e) {
            // continue
        }

        try {
            var JsPDF = getJsPdfCtor();
            if (JsPDF && JsPDF.API && JsPDF.API.events) {
                for (var i = 0; i < JsPDF.API.events.length; i++) {
                    var entry = JsPDF.API.events[i];
                    if (entry[0] === "addFonts" && typeof entry[1] === "function") {
                        entry[1].call(doc);
                    }
                }
            }
        } catch (e2) {
            return false;
        }

        try {
            return !!(doc.getFontList && doc.getFontList()[PDF_UNICODE_FONT]);
        } catch (e3) {
            return false;
        }
    }

    function getPdfLabels(isArabic) {
        if (isArabic) {
            return {
                governorate: "محافظة جنوب الباطنة",
                labelDate: "التاريخ",
                labelTime: "الوقت",
                labelName: "الاسم",
                labelEmployeeCode: "رمز الموظف",
                labelFilterDate: "تاريخ التقرير",
                labelPrintedBy: "تمت الطباعة بواسطة"
            };
        }

        return {
            governorate: "Al Batinah South Governorate",
            labelDate: "Date",
            labelTime: "Time",
            labelName: "Name",
            labelEmployeeCode: "Employee Code",
            labelFilterDate: "Date",
            labelPrintedBy: "Report Printed By"
        };
    }

    function pad2(num) {
        return (num < 10 ? "0" : "") + num;
    }

    function getDateTimeParts() {
        var now = new Date();
        return {
            date: pad2(now.getDate()) + "/" + pad2(now.getMonth() + 1) + "/" + now.getFullYear(),
            time: pad2(now.getHours()) + ":" + pad2(now.getMinutes()) + ":" + pad2(now.getSeconds())
        };
    }

    function pickFirst(root, selectors) {
        for (var i = 0; i < selectors.length; i++) {
            var node = root.querySelector(selectors[i]);
            if (node) return node;
        }
        return null;
    }

    function isPlaceholderValue(value) {
        if (value == null) return true;
        var normalized = String(value).replace(/\s+/g, " ").trim();
        if (!normalized) return true;
        var lower = normalized.toLowerCase();
        return lower === "n/a" || lower === "na" || lower === "null" || lower === "undefined";
    }

    function cleanTextValue(value) {
        if (value == null) return "";
        return String(value).replace(/\s+/g, " ").trim();
    }

    function getCurrentUserName() {
        if (typeof window.userRole === "string" && window.userRole.trim()) {
            var fromRole = window.userRole.replace(/^(HR|EMPLOYEE|LINE MANAGER)\s*:\s*/i, "").trim();
            if (!isPlaceholderValue(fromRole) && fromRole.toLowerCase() !== "unauthorized" && !/^\d+$/.test(fromRole)) {
                return fromRole;
            }
        }

        var fromPageTitle = document.querySelector(".page-title .text-success");
        if (fromPageTitle) {
            var fromPageTitleText = cleanTextValue(fromPageTitle.textContent || "");
            if (!isPlaceholderValue(fromPageTitleText) && !/^\d+$/.test(fromPageTitleText)) {
                return fromPageTitleText;
            }
        }

        var navCandidates = [
            ".nav-user span",
            "a.nav-user span",
            ".navbar-custom .nav-user",
            "a[href*='UserProfile'] span"
        ];
        for (var i = 0; i < navCandidates.length; i++) {
            var navUser = document.querySelector(navCandidates[i]);
            if (!navUser) continue;
            var navText = cleanTextValue(navUser.textContent || "");
            if (!isPlaceholderValue(navText)) {
                navText = navText.replace(/^(HR|EMPLOYEE|LINE MANAGER)\s*:\s*/i, "").trim();
                if (!isPlaceholderValue(navText) && !/^\d+$/.test(navText)) return navText;
            }
        }

        var sidebarUserNode = document.querySelector("#sidebar-menu .mdi-account-box + span");
        if (sidebarUserNode) {
            var sidebarUser = cleanTextValue(sidebarUserNode.textContent || "");
            if (!isPlaceholderValue(sidebarUser) && !/^\d+$/.test(sidebarUser)) {
                return sidebarUser;
            }
        }

        var titleText = (document.querySelector(".page-title") && document.querySelector(".page-title").textContent || "").trim();
        if (titleText) {
            var m = titleText.match(/Name\s*:\s*([^\]\n\r]+)/i);
            if (m && m[1] && !isPlaceholderValue(m[1]) && !/^\d+$/.test(m[1].trim())) return m[1].trim();
        }

        return "N/A";
    }

    function getCurrentUserCode() {
        if (typeof window.userRole === "string" && window.userRole.trim()) {
            var fromRole = window.userRole.replace(/^(HR|EMPLOYEE|LINE MANAGER)\s*:\s*/i, "").trim();
            if (fromRole && fromRole.toLowerCase() !== "unauthorized") {
                return fromRole;
            }
        }

        var loginIdNode = document.querySelector(".page-title .text-warning");
        if (loginIdNode) {
            var fromText = (loginIdNode.textContent || "").trim();
            if (!isPlaceholderValue(fromText)) return fromText;
            var fromTitle = (loginIdNode.getAttribute("title") || "").trim();
            if (!isPlaceholderValue(fromTitle)) return fromTitle;
        }

        var sidebarCodeNode = document.querySelector("#sidebar-menu .mdi-account-box + span");
        if (sidebarCodeNode) {
            var sidebarCode = cleanTextValue(sidebarCodeNode.textContent || "");
            if (!isPlaceholderValue(sidebarCode)) {
                return sidebarCode;
            }
        }

        var titleText = (document.querySelector(".page-title") && document.querySelector(".page-title").textContent || "").trim();
        if (titleText) {
            var m = titleText.match(/Login ID\s*:\s*([^\]&\n\r]+)/i);
            if (m && m[1] && !isPlaceholderValue(m[1])) return m[1].trim();
        }
        return "N/A";
    }

    function getChosenSelectedText(selectNode) {
        if (!selectNode || !selectNode.id) return "";
        var chosen = document.querySelector("#" + selectNode.id + "_chosen .chosen-single span");
        if (chosen && (chosen.textContent || "").trim()) {
            return (chosen.textContent || "").trim();
        }
        return "";
    }

    function getTableHeaderIndex(table, keywords) {
        if (!table) return -1;
        var ths = table.querySelectorAll("thead th");
        for (var i = 0; i < ths.length; i++) {
            var h = ((ths[i].textContent || "").trim()).toLowerCase();
            for (var k = 0; k < keywords.length; k++) {
                if (h.indexOf(keywords[k]) > -1) return i;
            }
        }
        return -1;
    }

    function getFirstBodyCellByIndex(table, idx) {
        if (!table || idx < 0) return "";
        var firstRow = table.querySelector("tbody tr");
        if (!firstRow) return "";
        var cells = firstRow.querySelectorAll("td");
        if (!cells || cells.length <= idx) return "";
        return (cells[idx].textContent || "").trim();
    }

    function getEmployeeNameFromTable(table) {
        if (!table) return "";
        var firstNameIdx = getTableHeaderIndex(table, ["fname", "first name", "employee first name", "الاسم"]);
        var lastNameIdx = getTableHeaderIndex(table, ["lname", "last name", "employee last name"]);
        var fullNameIdx = getTableHeaderIndex(table, ["employee name", "emp name", "full name", "employee full name"]);

        var firstName = getFirstBodyCellByIndex(table, firstNameIdx);
        var lastName = getFirstBodyCellByIndex(table, lastNameIdx);
        if (firstName || lastName) {
            return (firstName + " " + lastName).replace(/\s+/g, " ").trim();
        }

        return getFirstBodyCellByIndex(table, fullNameIdx);
    }

    function getEmployeeName(contextRoot, table) {
        var root = contextRoot || document;
        var selectors = [
            "#EmployeeName",
            "#employee_name",
            "#employees option:checked",
            "#monthly-time-sheet-employees option:checked",
            "#consolidated-employees option:checked",
            "#monthly-punched-photo-employees option:checked",
            "#monthly-geo-phencing-employees option:checked",
            "#monthly-captured-photo-employees option:checked",
            "#EmployeeId option:checked",
            "#employee_id option:checked",
            "select[name='employee_code'] option:checked",
            "select[name='employee_id'] option:checked",
            "select[name='employee_d'] option:checked"
        ];
        for (var i = 0; i < selectors.length; i++) {
            var node = root.querySelector(selectors[i]) || document.querySelector(selectors[i]);
            if (!node) continue;
            var value = (node.textContent || node.value || "").trim();
            if (value && value.toLowerCase() !== "all" && value.toLowerCase() !== "select employee") {
                if (value.indexOf(" - ") > -1) {
                    return value.split(" - ").slice(1).join(" - ").trim() || value;
                }
                return value;
            }
        }
        var selected = pickFirst(root, ["select[name='employee_id']", "select[name='employee_d']"]) || document.querySelector("select[name='employee_id'], select[name='employee_d']");
        if (selected && selected.selectedOptions && selected.selectedOptions.length > 0) {
            var selectedText = (selected.selectedOptions[0].textContent || "").trim();
            if (selectedText && selectedText.toLowerCase() !== "all") {
                if (selectedText.indexOf(" - ") > -1) {
                    return selectedText.split(" - ").slice(1).join(" - ").trim() || selectedText;
                }
                return selectedText;
            }
        }

        if (selected) {
            var chosenText = getChosenSelectedText(selected);
            if (chosenText && chosenText.toLowerCase() !== "all") {
                if (chosenText.indexOf(" - ") > -1) {
                    return chosenText.split(" - ").slice(1).join(" - ").trim() || chosenText;
                }
                return chosenText;
            }
        }

        var nameFromTable = getEmployeeNameFromTable(table);
        if (nameFromTable) return nameFromTable;

        return getCurrentUserName();
    }

    function getEmployeeCode(contextRoot, table) {
        var root = contextRoot || document;
        var codeInput = pickFirst(root, ["#EmployeeCode", "#employee_code"]) || document.querySelector("#EmployeeCode, #employee_code");
        if (codeInput && (codeInput.value || "").trim()) {
            return (codeInput.value || "").trim();
        }

        var selectedOptionSelectors = [
            "#monthly-time-sheet-employees option:checked",
            "#consolidated-employees option:checked",
            "#monthly-punched-photo-employees option:checked",
            "#monthly-geo-phencing-employees option:checked",
            "#monthly-captured-photo-employees option:checked",
            "#employees option:checked",
            "#employee_id option:checked",
            "select[name='employee_code'] option:checked",
            "select[name='employee_id'] option:checked",
            "select[name='employee_d'] option:checked"
        ];

        for (var s = 0; s < selectedOptionSelectors.length; s++) {
            var opt = root.querySelector(selectedOptionSelectors[s]) || document.querySelector(selectedOptionSelectors[s]);
            if (!opt) continue;
            var text = (opt.textContent || "").trim();
            if (!text) continue;
            if (text.indexOf(" - ") > -1) {
                return text.split(" - ")[0].trim();
            }
        }

        var selectors = [
            "#monthly-time-sheet-employees",
            "#consolidated-employees",
            "#monthly-punched-photo-employees",
            "#monthly-geo-phencing-employees",
            "#monthly-captured-photo-employees",
            "#employees",
            "#EmployeeId",
            "#employee_id",
            "select[name='employee_code']",
            "select[name='employee_id']",
            "select[name='employee_d']"
        ];
        for (var i = 0; i < selectors.length; i++) {
            var node = root.querySelector(selectors[i]) || document.querySelector(selectors[i]);
            if (!node) continue;
            var value = (node.value || "").trim();
            if (value && value !== "-1" && value !== "0") {
                return value;
            }
        }

        var selected = pickFirst(root, ["select[name='employee_id']", "select[name='employee_d']"]) || document.querySelector("select[name='employee_id'], select[name='employee_d']");
        if (selected) {
            var chosenText = getChosenSelectedText(selected);
            if (chosenText) {
                if (chosenText.indexOf(" - ") > -1) return chosenText.split(" - ")[0].trim();
                return chosenText;
            }
        }

        var codeIdx = getTableHeaderIndex(table, ["empcode", "employee code", "employeecode", "رمز الموظف", "رمز"]);
        var codeFromTable = getFirstBodyCellByIndex(table, codeIdx);
        if (codeFromTable) return codeFromTable;

        return getCurrentUserCode();
    }

    function parseMonthValue(monthValue) {
        if (!monthValue) return null;
        var normalized = monthValue.trim();
        var parts = normalized.split("-");
        if (parts.length !== 2) return null;

        var year;
        var month;

        if (parts[0].length === 4) {
            year = parseInt(parts[0], 10);
            month = parseInt(parts[1], 10);
        } else if (parts[1].length === 4) {
            month = parseInt(parts[0], 10);
            year = parseInt(parts[1], 10);
        } else {
            return null;
        }

        if (!year || !month || month < 1 || month > 12) return null;
        var lastDay = new Date(year, month, 0).getDate();
        return {
            from: "01/" + pad2(month) + "/" + year,
            to: pad2(lastDay) + "/" + pad2(month) + "/" + year
        };
    }

    function getFilterDateRange(contextRoot) {
        var root = contextRoot || document;
        var fromInput = pickFirst(root, ["[name='from_date']", "[name='fromDate']", "#date_from", "#from-date", "#from_date"]) || document.querySelector("[name='from_date'], [name='fromDate'], #date_from, #from-date, #from_date");
        var toInput = pickFirst(root, ["[name='to_date']", "[name='toDate']", "#date_to", "#to-date", "#to_date"]) || document.querySelector("[name='to_date'], [name='toDate'], #date_to, #to-date, #to_date");
        var fromValue = fromInput && (fromInput.value || "").trim();
        var toValue = toInput && (toInput.value || "").trim();
        if (fromValue && toValue) {
            return fromValue + " to " + toValue;
        }

        var reviewPeriodNode = pickFirst(root, ["#_reviewPeriod", "[name='reviewPeriod']"]) || document.querySelector("#_reviewPeriod, [name='reviewPeriod']");
        if (reviewPeriodNode) {
            var reviewValue = (reviewPeriodNode.value || "").trim();
            var reviewText = "";
            if (reviewPeriodNode.selectedOptions && reviewPeriodNode.selectedOptions.length > 0) {
                reviewText = (reviewPeriodNode.selectedOptions[0].textContent || "").trim();
            }

            if (reviewValue && reviewValue.toLowerCase() === "today") {
                var now = getDateTimeParts();
                return now.date;
            }

            if (reviewText && reviewText.toLowerCase() !== "none") {
                return reviewText;
            }
        }

        function firstNonEmptyValue(selectors) {
            for (var i = 0; i < selectors.length; i++) {
                var local = root.querySelector(selectors[i]);
                var localValue = local && (local.value || "").trim();
                if (localValue) return localValue;
            }
            for (var j = 0; j < selectors.length; j++) {
                var global = document.querySelector(selectors[j]);
                var globalValue = global && (global.value || "").trim();
                if (globalValue) return globalValue;
            }
            return "";
        }

        var monthValue = firstNonEmptyValue([
            "#monthly-time-sheet-month",
            "#month_year",
            "[name='month_year']",
            "[name='lmonth_year']",
            "#pdf-month",
            "[name='month']"
        ]);
        if (monthValue) {
            var parsed = parseMonthValue(monthValue);
            if (parsed) {
                return parsed.from + " to " + parsed.to;
            }
            return monthValue;
        }

        var anyDate = pickFirst(root, ["input[type='date']", "input.datepicker", "input[name*='date']", "input[name*='month']"]);
        if (anyDate && (anyDate.value || "").trim()) {
            return (anyDate.value || "").trim();
        }

        var fallbackToday = getDateTimeParts();
        return fallbackToday.date;
    }

    function loadImageAsDataUrl(src, callback) {
        var img = new Image();
        img.crossOrigin = "anonymous";
        img.onload = function () {
            try {
                var canvas = document.createElement("canvas");
                canvas.width = img.naturalWidth || img.width;
                canvas.height = img.naturalHeight || img.height;
                var ctx = canvas.getContext("2d");
                ctx.drawImage(img, 0, 0);
                callback(canvas.toDataURL("image/png"));
            } catch (err) {
                callback(null);
            }
        };
        img.onerror = function () {
            callback(null);
        };
        img.src = src;
    }

    function getConfiguredLogoPath() {
        var cfg = window.ExportTConfig || {};
        var p = (cfg.logoPath || "").trim();
        if (!p) {
            return window.location.origin + "/Content/Logos/logo-default.png";
        }

        if (p.indexOf("http://") === 0 || p.indexOf("https://") === 0 || p.indexOf("data:") === 0) {
            return p;
        }
        if (p.indexOf("~/") === 0) {
            return window.location.origin + p.substring(1);
        }
        if (p.indexOf("/") === 0) {
            return window.location.origin + p;
        }
        return window.location.origin + "/" + p;
    }

    function getPrintedByName() {
        var printedBy = getCurrentUserName();
        if (printedBy && printedBy !== "N/A") {
            return printedBy;
        }

        var loginId = getCurrentUserCode();
        if (loginId && loginId !== "N/A") {
            return loginId;
        }

        return "System";
    }

    function getHeaderName() {
        var userName = getCurrentUserName();
        if (userName && userName !== "N/A") {
            return userName;
        }

        // Keep header name populated with login identity when full name is unavailable.
        return getPrintedByName();
    }

    function setPdfFont(doc, useUnicodeFont, isBold) {
        if (useUnicodeFont) {
            try {
                doc.setFont(PDF_UNICODE_FONT, "normal");
                return;
            } catch (e) {
                doc.setFont("helvetica", isBold ? "bold" : "normal");
                return;
            }
        }
        doc.setFont("helvetica", isBold ? "bold" : "normal");
    }

    function drawPdfHeader(doc, reportTitle, headerInfo, labels, isArabic, useUnicodeFont, done) {
        var pageWidth = doc.internal.pageSize.getWidth();
        var leftX = 14;
        var rightX = pageWidth - 14;
        var centerX = pageWidth / 2;
        var logoPath = getConfiguredLogoPath();

        setPdfFont(doc, useUnicodeFont, false);
        doc.setFontSize(11);
        if (isArabic) {
            doc.text(labels.governorate, rightX, 12, { align: "right" });
        } else {
            doc.text(labels.governorate, leftX, 12);
        }

        doc.setFontSize(10);
        if (isArabic) {
            doc.text(labels.labelDate + " : " + headerInfo.date, leftX, 10);
            doc.text(labels.labelTime + " : " + headerInfo.time, leftX, 16);
        } else {
            doc.text(labels.labelDate + " : " + headerInfo.date, rightX, 10, { align: "right" });
            doc.text(labels.labelTime + " : " + headerInfo.time, rightX, 16, { align: "right" });
        }

        loadImageAsDataUrl(logoPath, function (logoData) {
            if (logoData) {
                doc.addImage(logoData, "PNG", centerX - 10, 4, 20, 20);
            }

            doc.setLineWidth(0.3);
            doc.line(leftX, 24, rightX, 24);

            setPdfFont(doc, useUnicodeFont, false);
            doc.setFontSize(10);
            if (isArabic) {
                doc.text(labels.labelEmployeeCode + ": " + headerInfo.employeeCode, rightX, 31, { align: "right" });
                doc.text(labels.labelFilterDate + ": " + headerInfo.filterDateRange, rightX, 37, { align: "right" });
                doc.text(reportTitle, leftX, 34);
            } else {
                doc.text(labels.labelEmployeeCode + ": " + headerInfo.employeeCode, leftX, 31);
                doc.text(labels.labelFilterDate + ": " + headerInfo.filterDateRange, leftX, 37);
                doc.text(reportTitle, rightX, 34, { align: "right" });
            }

            done(50);
        });
    }

    function exportVisibleTableAsPdf(done, triggerElement) {
        if (!hasJsPdf()) {
            alert("PDF library not loaded.");
            done(false);
            return;
        }

        var table = closestVisibleTable();
        if (!table) {
            alert("No report data found to export.");
            done(false);
            return;
        }

        var jsPDF = getJsPdfCtor();
        var doc = new jsPDF("l", "mm", "a4");
        var unicodeFontReady = registerPdfUnicodeFonts(doc);
        var isArabic = isArabicLanguage();
        var labels = getPdfLabels(isArabic);
        var title = getReportTitle();
        var dateTime = getDateTimeParts();
        var contextRoot = (triggerElement && (triggerElement.form || triggerElement.closest("form"))) || document;
        var headerInfo = {
            date: dateTime.date,
            time: dateTime.time,
            employeeName: getHeaderName(),
            employeeCode: getEmployeeCode(contextRoot, table),
            filterDateRange: getFilterDateRange(contextRoot)
        };

        if (!headerInfo.employeeName || headerInfo.employeeName === "N/A") {
            headerInfo.employeeName = getHeaderName();
        }
        if (!headerInfo.employeeCode || headerInfo.employeeCode === "N/A") {
            headerInfo.employeeCode = getCurrentUserCode();
        }
        if (!headerInfo.filterDateRange || headerInfo.filterDateRange === "N/A") {
            headerInfo.filterDateRange = getDateTimeParts().date;
        }

        if (headerInfo.employeeName && headerInfo.employeeCode && headerInfo.employeeName === headerInfo.employeeCode) {
            var nonCodeName = getCurrentUserName();
            if (nonCodeName && nonCodeName !== "N/A" && nonCodeName !== headerInfo.employeeCode) {
                headerInfo.employeeName = nonCodeName;
            }
        }

        var printedBy = getPrintedByName();
        title = normalizePdfText(title);
        headerInfo.employeeName = normalizePdfText(headerInfo.employeeName);
        headerInfo.employeeCode = normalizePdfText(headerInfo.employeeCode);
        headerInfo.filterDateRange = normalizePdfText(headerInfo.filterDateRange);
        printedBy = normalizePdfText(printedBy);

        var needsUnicodeFont = shouldUseUnicodeFont(isArabic, table, [
            title,
            headerInfo.employeeName,
            headerInfo.employeeCode,
            headerInfo.filterDateRange,
            printedBy
        ]);
        var useUnicodeFont = unicodeFontReady;

        if (needsUnicodeFont && !unicodeFontReady) {
            alert("Arabic PDF font could not be loaded. Please refresh the page and try again.");
            done(false);
            return;
        }

        setPdfFont(doc, useUnicodeFont, false);

        if (typeof doc.autoTable !== "function" && window.jspdfAutoTable) {
            doc.autoTable = window.jspdfAutoTable.default || window.jspdfAutoTable;
        }

        if (typeof doc.autoTable !== "function") {
            alert("PDF table plugin not loaded.");
            done(false);
            return;
        }

        drawPdfHeader(doc, title, headerInfo, labels, isArabic, useUnicodeFont, function (startY) {
            function cellText(el) {
                return normalizePdfText(el.textContent || "");
            }

            function normalizeCellText(cell) {
                if (cell == null) return cell;
                if (typeof cell === "string") {
                    return normalizePdfText(cell);
                }
                if (Array.isArray(cell)) {
                    for (var ci = 0; ci < cell.length; ci++) {
                        cell[ci] = normalizePdfText(cell[ci]);
                    }
                }
                return cell;
            }

            function extractTableMatrix(tbl) {
                var head = [];
                var theadRows = tbl.querySelectorAll("thead tr");
                for (var hr = 0; hr < theadRows.length; hr++) {
                    var hcells = theadRows[hr].querySelectorAll("th, td");
                    var row = [];
                    for (var hc = 0; hc < hcells.length; hc++) {
                        row.push(cellText(hcells[hc]));
                    }
                    if (row.length) head.push(row);
                }

                var body = [];
                var bodyRows = tbl.querySelectorAll("tbody tr");
                for (var br = 0; br < bodyRows.length; br++) {
                    var bcells = bodyRows[br].querySelectorAll("td");
                    var brow = [];
                    for (var bc = 0; bc < bcells.length; bc++) {
                        brow.push(cellText(bcells[bc]));
                    }
                    if (brow.length) body.push(brow);
                }

                return { head: head, body: body };
            }

            function reverseMatrixRows(rows) {
                var out = [];
                for (var i = 0; i < rows.length; i++) {
                    out.push(rows[i].slice().reverse());
                }
                return out;
            }

            var pdfTableFont = useUnicodeFont ? PDF_UNICODE_FONT : "helvetica";
            var autoTableOpts = {
                startY: startY,
                theme: "grid",
                styles: {
                    fontSize: 8,
                    cellPadding: 1.5,
                    overflow: "linebreak",
                    halign: isArabic ? "right" : "left",
                    font: pdfTableFont,
                    fontStyle: "normal"
                },
                headStyles: {
                    fillColor: [52, 58, 64],
                    textColor: 255,
                    halign: isArabic ? "right" : "left",
                    font: pdfTableFont,
                    fontStyle: "normal"
                },
                didParseCell: function (data) {
                    if (data.cell && data.cell.text != null) {
                        data.cell.text = normalizeCellText(data.cell.text);
                    }
                    if (useUnicodeFont) {
                        data.cell.styles.font = PDF_UNICODE_FONT;
                        data.cell.styles.fontStyle = "normal";
                    }
                },
                willDrawCell: function (data) {
                    if (useUnicodeFont) {
                        doc.setFont(PDF_UNICODE_FONT, "normal");
                    }
                }
            };

            var matrix = extractTableMatrix(table);
            if (matrix.head.length && matrix.body.length) {
                autoTableOpts.head = isArabic ? reverseMatrixRows(matrix.head) : matrix.head;
                autoTableOpts.body = isArabic ? reverseMatrixRows(matrix.body) : matrix.body;
            } else {
                autoTableOpts.html = table;
            }

            doc.autoTable(autoTableOpts);

            var y = (doc.lastAutoTable && doc.lastAutoTable.finalY ? doc.lastAutoTable.finalY : startY) + 10;
            if (y > 200) {
                doc.addPage();
                y = 20;
            }
            setPdfFont(doc, useUnicodeFont, true);
            if (isArabic) {
                doc.text(labels.labelPrintedBy + ": " + printedBy, doc.internal.pageSize.getWidth() - 14, y, { align: "right" });
            } else {
                doc.text(labels.labelPrintedBy + ": " + printedBy, 14, y);
            }

            doc.save(buildFileName(title, isArabic));
            done(true);
        });
    }

    function shouldHandlePdfButton(el) {
        if (!el || !el.id) {
            return false;
        }

        if (el.id === "generate-payroll") {
            return false;
        }

        if (PDF_BUTTON_IDS.indexOf(el.id) >= 0) {
            return true;
        }

        if (el.id.indexOf("generate-") === 0) {
            var text = (el.textContent || el.value || "").toLowerCase();
            if (text.indexOf("pdf") >= 0 || text.indexOf("statement") >= 0 || text.indexOf("download") >= 0) {
                return true;
            }
        }

        return false;
    }

    document.addEventListener("click", function (e) {
        var button = e.target.closest("button, input[type='submit']");
        if (!button || !shouldHandlePdfButton(button)) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();
        exportVisibleTableAsPdf(function () { }, button);
    }, true);

    window.ExportT = {
        exportVisibleTableAsPdf: exportVisibleTableAsPdf
    };
})(window, document);
