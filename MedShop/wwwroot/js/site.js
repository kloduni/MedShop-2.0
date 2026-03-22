document.addEventListener('DOMContentLoaded', () => {
    const addToCartButtons = document.querySelectorAll('.add-to-cart-btn');

    addToCartButtons.forEach(button => {
        button.addEventListener('click', async (e) => {
            e.preventDefault();
            e.stopPropagation();

            const productId = button.getAttribute('data-id');
            const originalText = button.innerHTML;

            // Show loading state
            button.innerHTML = '<i class="bi bi-hourglass-split"></i> Adding...';
            button.disabled = true;

            try {
                // Call the new JSON endpoint with the explicit query parameter
                const response = await fetch(`/ShoppingCart/AddItemJson?id=${productId}`, {
                    method: 'POST'
                });

                // Grab the raw text response first to handle non-OK server statuses
                const text = await response.text();

                if (!response.ok) {
                    console.error("Server Error:", response.status, text);
                    toastr.error(`Server rejected the request (Error ${response.status}).`, 'Error');
                    button.innerHTML = originalText;
                    button.disabled = false;
                    return;
                }

                const data = JSON.parse(text);

                if (data.success) {
                    // Visual Success!
                    button.innerHTML = '<i class="bi bi-check-circle-fill"></i> Added!';
                    button.classList.replace('bg-blue-600', 'bg-emerald-600');
                    button.classList.replace('hover:bg-blue-500', 'hover:bg-emerald-500'); // <-- Add this!

                    // Update the Cart Notification Badge
                    const badge = document.getElementById('cart-badge-count');
                    if (badge) {
                        badge.innerText = data.cartCount;
                        badge.classList.remove('hidden');
                    }

                    // Reset button back to blue after 2 seconds
                    setTimeout(() => {
                        button.innerHTML = originalText;
                        button.classList.replace('bg-emerald-600', 'bg-blue-600');
                        button.classList.replace('hover:bg-emerald-500', 'hover:bg-blue-500'); // <-- Add this!
                        button.disabled = false;
                    }, 2000);
                } else {

                    // Change the button to a Red Error State
                    button.innerHTML = '<i class="bi bi-x-circle-fill"></i> Unavailable';
                    button.classList.replace('bg-blue-600', 'bg-red-600');
                    button.classList.replace('hover:bg-blue-500', 'hover:bg-red-500'); // <-- Add this!

                    // Show friendly warning using Toastr if there's a specific message
                    if (data.message) {
                        toastr.warning(data.message, 'Notice');
                    }

                    // Reset the button back to normal (Blue) after 2 seconds
                    setTimeout(() => {
                        button.innerHTML = originalText;
                        button.classList.replace('bg-red-600', 'bg-blue-600');
                        button.classList.replace('hover:bg-red-500', 'hover:bg-blue-500'); // <-- Add this!
                        button.disabled = false;
                    }, 2000);
                }
            } catch (error) {
                console.error("Network or Fetch error:", error);
                toastr.error("A network error occurred. Check the console.", 'Error');
                button.innerHTML = originalText;
                button.disabled = false;
            }
        });
    });
});