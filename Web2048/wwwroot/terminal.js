let term = null;

function setupTerminal() {
    const terminalElement = document.getElementById('terminal');
    if (terminalElement) {
        term = new Terminal({
            cols: 80,
            rows: 40,
            scrollback: 0,
            disableStdin: false,
            windowOption: {
                fillScreenResize: true
            },
            fontSize: 20
        });
        term.open(terminalElement);
    } else {
        console.error('Terminal container not found!');
    }
}

setupTerminal();

function getAnsiColor(consoleColor) {
    switch(consoleColor) {
        case 0:  // Black
            return '\x1b[30m';
        case 1:  // DarkBlue
            return '\x1b[34m';
        case 2:  // DarkGreen
            return '\x1b[32m';
        case 3:  // DarkCyan
            return '\x1b[36m';
        case 4:  // DarkRed
            return '\x1b[31m';
        case 5:  // DarkMagenta
            return '\x1b[35m';
        case 6:  // DarkYellow
            return '\x1b[33m';
        case 7:  // Gray
            return '\x1b[37m';
        case 8:  // DarkGray
            return '\x1b[90m';
        case 9:  // Blue
            return '\x1b[94m';
        case 10: // Green
            return '\x1b[92m';
        case 11: // Cyan
            return '\x1b[96m';
        case 12: // Red
            return '\x1b[91m';
        case 13: // Magenta
            return '\x1b[95m';
        case 14: // Yellow
            return '\x1b[93m';
        case 15: // White
            return '\x1b[97m';
        default:
            return '\x1b[0m'; // Reset
    }
}

window.consoleEmulator = {
    write: (text) => {
        term.write(text);
    },
    writeColor: (text, color) => {
        const colorCode = getAnsiColor(color);
        term.write(`${colorCode}${text}\x1b[0m`);
    },
    clear: () => {
        term.clear();
    },
    setCursor: (x, y) => {
        term.write(`\x1b[${y + 1};${x + 1}H`);
    }
};

export function setupTerminalKeyListener(dotNetHelper) {
    if (term) {
        term.onKey(({ key, domEvent }) => {
            dotNetHelper.invokeMethodAsync('HandleKeyPress', domEvent.keyCode);
        });
    } else {
        console.error('Terminal not initialized');
    }
}