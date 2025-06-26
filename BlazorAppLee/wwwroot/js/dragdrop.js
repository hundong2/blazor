// JSON Puzzle Editor JavaScript Interop

window.puzzleEditor = {
    // Initialize the puzzle editor
    init: function() {
        console.log('Puzzle Editor initialized');
    },

    // Start dragging a piece
    startDrag: function(pieceId) {
        const piece = document.getElementById(pieceId);
        if (piece) {
            piece.style.zIndex = '1000';
            piece.classList.add('dragging');
        }
    },

    // End dragging a piece
    endDrag: function(pieceId) {
        const piece = document.getElementById(pieceId);
        if (piece) {
            piece.style.zIndex = '';
            piece.classList.remove('dragging');
        }
    },

    // Get pointer position relative to canvas
    getCanvasPosition: function(canvasId, clientX, clientY) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return { x: 0, y: 0 };

        const rect = canvas.getBoundingClientRect();
        return {
            x: clientX - rect.left,
            y: clientY - rect.top
        };
    },

    // Add event listeners for drag and drop
    setupDragEvents: function(dotnetRef, canvasId) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        let isDragging = false;
        let draggedElement = null;

        // Unified pointer event handling
        const getPointerPos = (e) => {
            const touch = e.touches ? e.touches[0] : e;
            return this.getCanvasPosition(canvasId, touch.clientX, touch.clientY);
        };

        // Pointer down (mouse/touch start)
        const handlePointerDown = (e) => {
            const target = e.target.closest('.puzzle-piece');
            if (!target) return;

            e.preventDefault();
            const pos = getPointerPos(e);
            const pieceId = target.id;
            
            draggedElement = target;
            isDragging = true;

            dotnetRef.invokeMethodAsync('OnPointerDown', pieceId, pos.x, pos.y);
        };

        // Pointer move (mouse/touch move)
        const handlePointerMove = (e) => {
            if (!isDragging || !draggedElement) return;

            e.preventDefault();
            const pos = getPointerPos(e);
            dotnetRef.invokeMethodAsync('OnPointerMove', pos.x, pos.y);
        };

        // Pointer up (mouse/touch end)
        const handlePointerUp = (e) => {
            if (!isDragging) return;

            e.preventDefault();
            isDragging = false;
            draggedElement = null;

            dotnetRef.invokeMethodAsync('OnPointerUp');
        };

        // Mouse events
        canvas.addEventListener('mousedown', handlePointerDown);
        document.addEventListener('mousemove', handlePointerMove);
        document.addEventListener('mouseup', handlePointerUp);

        // Touch events
        canvas.addEventListener('touchstart', handlePointerDown, { passive: false });
        document.addEventListener('touchmove', handlePointerMove, { passive: false });
        document.addEventListener('touchend', handlePointerUp, { passive: false });

        // Prevent context menu on long press
        canvas.addEventListener('contextmenu', (e) => e.preventDefault());

        // Store cleanup function
        canvas._puzzleCleanup = () => {
            canvas.removeEventListener('mousedown', handlePointerDown);
            document.removeEventListener('mousemove', handlePointerMove);
            document.removeEventListener('mouseup', handlePointerUp);
            canvas.removeEventListener('touchstart', handlePointerDown);
            document.removeEventListener('touchmove', handlePointerMove);
            document.removeEventListener('touchend', handlePointerUp);
            canvas.removeEventListener('contextmenu', (e) => e.preventDefault());
        };
    },

    // Cleanup event listeners
    cleanup: function(canvasId) {
        const canvas = document.getElementById(canvasId);
        if (canvas && canvas._puzzleCleanup) {
            canvas._puzzleCleanup();
            delete canvas._puzzleCleanup;
        }
    },

    // Create SVG path for connections
    createConnectionPath: function(fromX, fromY, toX, toY) {
        const dx = toX - fromX;
        const dy = toY - fromY;
        
        // Control points for curved line
        const cp1x = fromX + Math.abs(dx) * 0.5;
        const cp1y = fromY;
        const cp2x = toX - Math.abs(dx) * 0.5;
        const cp2y = toY;

        return `M ${fromX} ${fromY} C ${cp1x} ${cp1y}, ${cp2x} ${cp2y}, ${toX} ${toY}`;
    },

    // Copy text to clipboard
    copyToClipboard: function(text) {
        if (navigator.clipboard && window.isSecureContext) {
            return navigator.clipboard.writeText(text).then(() => true).catch(() => false);
        } else {
            // Fallback for older browsers
            const textArea = document.createElement('textarea');
            textArea.value = text;
            textArea.style.position = 'fixed';
            textArea.style.left = '-999999px';
            textArea.style.top = '-999999px';
            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();
            
            try {
                document.execCommand('copy');
                document.body.removeChild(textArea);
                return Promise.resolve(true);
            } catch (err) {
                document.body.removeChild(textArea);
                return Promise.resolve(false);
            }
        }
    },

    // Animate piece creation
    animatePieceCreation: function(pieceId) {
        const piece = document.getElementById(pieceId);
        if (piece) {
            piece.classList.add('new');
            setTimeout(() => {
                piece.classList.remove('new');
            }, 300);
        }
    },

    // Get element dimensions
    getElementDimensions: function(elementId) {
        const element = document.getElementById(elementId);
        if (!element) return { width: 0, height: 0 };

        const rect = element.getBoundingClientRect();
        return {
            width: rect.width,
            height: rect.height
        };
    },

    // Set focus to input
    focusInput: function(inputId) {
        const input = document.getElementById(inputId);
        if (input) {
            input.focus();
            input.select();
        }
    },

    // Scroll to element
    scrollToElement: function(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.scrollIntoView({ 
                behavior: 'smooth', 
                block: 'center',
                inline: 'center'
            });
        }
    }
};