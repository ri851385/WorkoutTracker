// Blazor WebAssembly's TimeZoneInfo.Local resolution has been unreliable on some
// mobile PWA runtimes, silently falling back to UTC. Reading the local time via the
// browser's own Date object sidesteps that entirely.
window.workoutTrackerGetLocalNow = () => {
    const d = new Date();
    const pad = (value, length = 2) => String(value).padStart(length, '0');
    const offsetMinutes = -d.getTimezoneOffset();
    const sign = offsetMinutes >= 0 ? '+' : '-';
    const absMinutes = Math.abs(offsetMinutes);
    const offset = `${sign}${pad(Math.floor(absMinutes / 60))}:${pad(absMinutes % 60)}`;
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}.${pad(d.getMilliseconds(), 3)}${offset}`;
};
