window.workoutTrackerDownload = (fileName, content, mimeType) => {
    const blob = new Blob([content], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

// Excel (Japanese locale) needs a UTF-8 BOM to open CSV files without mangling non-ASCII text.
window.workoutTrackerDownloadCsv = (fileName, csvContent) => {
    const bom = String.fromCharCode(0xFEFF);
    window.workoutTrackerDownload(fileName, bom + csvContent, "text/csv;charset=utf-8;");
};
