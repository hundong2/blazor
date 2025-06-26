window.puzzleEditor = {
    dragData: {
        isDragging: false,
        startX: 0,
        startY: 0,
        offsetX: 0,
        offsetY: 0,
        element: null
    },

    initializeDragDrop: function() {
        console.log('Puzzle Editor drag drop initialized');
    },

    startDrag: function(elementId, clientX, clientY) {
        const element = document.getElementById(elementId);
        if (!element) return;

        this.dragData.isDragging = true;
        this.dragData.element = element;
        this.dragData.startX = clientX;
        this.dragData.startY = clientY;
        
        const rect = element.getBoundingClientRect();
        this.dragData.offsetX = clientX - rect.left;
        this.dragData.offsetY = clientY - rect.top;

        element.style.cursor = 'grabbing';
        element.style.zIndex = '1000';
        
        document.addEventListener('pointermove', this.onDrag.bind(this));
        document.addEventListener('pointerup', this.endDrag.bind(this));
    },

    onDrag: function(e) {
        if (!this.dragData.isDragging || !this.dragData.element) return;

        e.preventDefault();
        
        const newX = e.clientX - this.dragData.offsetX;
        const newY = e.clientY - this.dragData.offsetY;
        
        this.dragData.element.style.setProperty('--x', newX + 'px');
        this.dragData.element.style.setProperty('--y', newY + 'px');
    },

    endDrag: function(e) {
        if (!this.dragData.isDragging) return;

        this.dragData.isDragging = false;
        
        if (this.dragData.element) {
            this.dragData.element.style.cursor = 'grab';
            this.dragData.element.style.zIndex = '';
            
            // Get final position
            const finalX = e.clientX - this.dragData.offsetX;
            const finalY = e.clientY - this.dragData.offsetY;
            
            // Notify Blazor component
            const pieceId = this.dragData.element.getAttribute('data-piece-id');
            if (pieceId && window.blazorPuzzleEditor) {
                window.blazorPuzzleEditor.invokeMethodAsync('UpdatePiecePosition', pieceId, finalX, finalY);
            }
        }

        document.removeEventListener('pointermove', this.onDrag.bind(this));
        document.removeEventListener('pointerup', this.endDrag.bind(this));
        
        this.dragData.element = null;
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.puzzleEditor.initializeDragDrop();
});