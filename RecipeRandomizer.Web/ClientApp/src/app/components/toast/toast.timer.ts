export function Timer(callback, delay) {
    let timerId: number, start: number, remaining: number = delay;

    this.pause = function () {
        window.clearTimeout(timerId);
        remaining -= Date.now() - start;
    };

    this.resume = function () {
        start = Date.now();
        window.clearTimeout(timerId);
        timerId = window.setInterval(callback, remaining);
    };
}
