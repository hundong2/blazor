// JSON Puzzle Editor JavaScript Interop Functions

window.jsonPuzzleEditor = {
    draggedElement: null,
    dragOffset: { x: 0, y: 0 },
    isDragging: false
};

// Initialize canvas for drag and drop
window.initializeCanvas = (canvasElement) => {
    console.log('Initializing canvas:', canvasElement);
    
    canvasElement.addEventListener('dragover', (e) => {
        e.preventDefault();
    });
    
    canvasElement.addEventListener('drop', (e) => {
        e.preventDefault();
        console.log('Drop event on canvas');
    });
};

// Initialize puzzle piece for dragging
window.initializePuzzlePiece = (pieceElement, dotNetRef) => {
    console.log('Initializing puzzle piece:', pieceElement);
    
    let isDragging = false;
    let startX = 0;
    let startY = 0;
    let initialX = 0;
    let initialY = 0;
    
    // Mouse events
    pieceElement.addEventListener('mousedown', (e) => {
        if (e.button !== 0) return; // Only left mouse button
        
        isDragging = true;
        startX = e.clientX;
        startY = e.clientY;
        
        const rect = pieceElement.getBoundingClientRect();
        const canvasRect = pieceElement.closest('.canvas').getBoundingClientRect();
        
        initialX = rect.left - canvasRect.left;
        initialY = rect.top - canvasRect.top;
        
        pieceElement.style.zIndex = '1000';
        document.body.style.userSelect = 'none';
        
        e.preventDefault();
    });
    
    document.addEventListener('mousemove', (e) => {
        if (!isDragging) return;
        
        const deltaX = e.clientX - startX;
        const deltaY = e.clientY - startY;
        
        const newX = Math.max(0, initialX + deltaX);
        const newY = Math.max(0, initialY + deltaY);
        
        pieceElement.style.left = newX + 'px';
        pieceElement.style.top = newY + 'px';
        
        // Update position in Blazor component
        dotNetRef.invokeMethodAsync('UpdatePosition', newX, newY);
    });
    
    document.addEventListener('mouseup', () => {
        if (isDragging) {
            isDragging = false;
            pieceElement.style.zIndex = '10';
            document.body.style.userSelect = '';
            
            // Notify Blazor component that dragging stopped
            dotNetRef.invokeMethodAsync('StopDragging');
        }
    });
    
    // Touch events for mobile support
    pieceElement.addEventListener('touchstart', (e) => {
        const touch = e.touches[0];
        startX = touch.clientX;
        startY = touch.clientY;
        
        const rect = pieceElement.getBoundingClientRect();
        const canvasRect = pieceElement.closest('.canvas').getBoundingClientRect();
        
        initialX = rect.left - canvasRect.left;
        initialY = rect.top - canvasRect.top;
        
        isDragging = true;
        pieceElement.style.zIndex = '1000';
        
        e.preventDefault();
    });
    
    pieceElement.addEventListener('touchmove', (e) => {
        if (!isDragging) return;
        
        const touch = e.touches[0];
        const deltaX = touch.clientX - startX;
        const deltaY = touch.clientY - startY;
        
        const newX = Math.max(0, initialX + deltaX);
        const newY = Math.max(0, initialY + deltaY);
        
        pieceElement.style.left = newX + 'px';
        pieceElement.style.top = newY + 'px';
        
        // Update position in Blazor component
        dotNetRef.invokeMethodAsync('UpdatePosition', newX, newY);
        
        e.preventDefault();
    });
    
    pieceElement.addEventListener('touchend', () => {
        if (isDragging) {
            isDragging = false;
            pieceElement.style.zIndex = '10';
            
            // Notify Blazor component that dragging stopped
            dotNetRef.invokeMethodAsync('StopDragging');
        }
    });
};

// Utility functions
window.showToast = (message) => {
    // Simple toast implementation
    const toast = document.createElement('div');
    toast.textContent = message;
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #28a745;
        color: white;
        padding: 10px 20px;
        border-radius: 4px;
        z-index: 10000;
        font-size: 14px;
        box-shadow: 0 2px 4px rgba(0,0,0,0.2);
        transition: opacity 0.3s ease;
    `;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.style.opacity = '0';
        setTimeout(() => {
            document.body.removeChild(toast);
        }, 300);
    }, 2000);
};

window.copyToClipboardFallback = (text) => {
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
        window.showToast('JSON copied to clipboard!');
    } catch (err) {
        window.showToast('Failed to copy JSON');
    }
    
    document.body.removeChild(textArea);
};