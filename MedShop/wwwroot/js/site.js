document.addEventListener('DOMContentLoaded', () => {

    // Add to Cart Logic
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
                // Call the JSON endpoint
                const response = await fetch(`/ShoppingCart/AddItemJson?id=${productId}`, {
                    method: 'POST'
                });

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
                    button.classList.replace('hover:bg-blue-500', 'hover:bg-emerald-500');

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
                        button.classList.replace('hover:bg-emerald-500', 'hover:bg-blue-500');
                        button.disabled = false;
                    }, 2000);
                } else {
                    // Change the button to a Red Error State
                    button.innerHTML = '<i class="bi bi-x-circle-fill"></i> Unavailable';
                    button.classList.replace('bg-blue-600', 'bg-red-600');
                    button.classList.replace('hover:bg-blue-500', 'hover:bg-red-500');

                    if (data.message) {
                        toastr.warning(data.message, 'Notice');
                    }

                    setTimeout(() => {
                        button.innerHTML = originalText;
                        button.classList.replace('bg-red-600', 'bg-blue-600');
                        button.classList.replace('hover:bg-red-500', 'hover:bg-blue-500');
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

    // Buy Now Logic (Add to Cart + Redirect)
    const buyNowButtons = document.querySelectorAll('.buy-now-btn');

    buyNowButtons.forEach(button => {
        button.addEventListener('click', async (e) => {
            e.preventDefault();
            e.stopPropagation();

            const productId = button.getAttribute('data-id');
            const originalText = button.innerHTML;

            button.innerHTML = '<i class="bi bi-hourglass-split"></i> Redirecting...';
            button.disabled = true;

            try {
                const response = await fetch(`/ShoppingCart/AddItemJson?id=${productId}`, {
                    method: 'POST'
                });

                if (response.ok) {
                    const data = await response.json();
                    if (data.success) {
                        // Success! Send them to the cart immediately
                        window.location.href = '/ShoppingCart/ShoppingCart';
                    } else {
                        toastr.warning(data.message || 'Item unavailable', 'Notice');
                        button.innerHTML = originalText;
                        button.disabled = false;
                    }
                } else {
                    toastr.error('Server error occurred.', 'Error');
                    button.innerHTML = originalText;
                    button.disabled = false;
                }
            } catch (error) {
                console.error("Fetch error:", error);
                toastr.error("Network error.", 'Error');
                button.innerHTML = originalText;
                button.disabled = false;
            }
        });
    });

    // Wishlist Toggle Logic
    const wishlistButtons = document.querySelectorAll('.wishlist-btn');

    wishlistButtons.forEach(button => {
        button.addEventListener('click', async (e) => {
            e.preventDefault();
            e.stopPropagation();

            const productId = button.getAttribute('data-id');
            const icon = button.querySelector('i');

            // Prevent spam-clicking
            button.disabled = true;

            try {
                // Hit our backend endpoint
                const response = await fetch(`/Wishlist/Toggle?productId=${productId}`, {
                    method: 'POST'
                });

                if (response.ok) {
                    const data = await response.json();

                    if (data.success) {
                        if (data.isAdded) {
                            // Item was added
                            icon.classList.remove('bi-heart');
                            icon.classList.add('bi-heart-fill', 'text-pink-500');
                            toastr.success('Added to your wishlist!', 'Saved');
                        } else {
                            // Item was removed
                            icon.classList.remove('bi-heart-fill', 'text-pink-500');
                            icon.classList.add('bi-heart');
                            toastr.info('Removed from your wishlist.', 'Updated');

                            // --- NEW FADE OUT MAGIC ---
                            // Check if we are on the Wishlist page by looking for the wrapper ID
                            const cardElement = document.getElementById(`wishlist-card-${productId}`);
                            if (cardElement) {
                                // Add Tailwind classes to shrink and fade
                                cardElement.classList.add('opacity-0', 'scale-95');
                                
                                // Wait 300ms for the animation, then wipe it from the DOM
                                setTimeout(() => {
                                    cardElement.remove();
                                    
                                    // Pro-move: If that was the last item, reload to show the "Empty" screen
                                    const remainingCards = document.querySelectorAll('[id^="wishlist-card-"]');
                                    if (remainingCards.length === 0) {
                                        window.location.reload();
                                    }
                                }, 300);
                            }
                            // --------------------------
                        }
                    } else {
                        toastr.error(data.message || 'Could not update wishlist.', 'Oops!');
                    }
                } else {
                    // If the server returns 401 Unauthorized, redirect to login
                    if (response.status === 401) {
                        window.location.href = '/Identity/Account/Login';
                    } else {
                        toastr.error('Server error occurred.', 'Error');
                    }
                }
            } catch (error) {
                console.error("Fetch error:", error);
                toastr.error("Network error.", 'Error');
            } finally {
                // Re-enable the button
                button.disabled = false;
            }
        });
    });

});


// Visibility Toggle Function (AJAX)
function toggleProductVisibility(e, productId, btnElement) {
    e.preventDefault();
    e.stopPropagation();

    const button = $(btnElement);
    const badge = button.find('.status-badge');
    const originalHtml = badge.html();

    // Check our attribute to see what page we are on
    const removeOnHide = button.attr('data-remove-on-hide') === 'true';
    const card = button.closest('.group'); // Find the main product card container

    badge.html('<i class="bi bi-arrow-repeat animate-spin"></i>');

    $.post('/Product/ToggleVisibility', { id: productId })
        .done(function (data) {
            if (data.success) {
                if (data.isVisible) {
                    badge.removeClass('bg-slate-600/50 text-slate-400 border-slate-600')
                        .addClass('bg-emerald-500/20 text-emerald-400 border-emerald-500/30')
                        .text('Visible');
                } else {
                    // If hidden AND we are on the Storefront, dynamically delete the card!
                    if (removeOnHide) {
                        card.fadeOut(300, function () {
                            $(this).remove();
                        });
                    } else {
                        // Otherwise, just update the badge
                        badge.removeClass('bg-emerald-500/20 text-emerald-400 border-emerald-500/30')
                            .addClass('bg-slate-600/50 text-slate-400 border-slate-600')
                            .text('Hidden');
                    }
                }
            }
        })
        .fail(function () {
            alert("Something went wrong. Could not update visibility.");
            badge.html(originalHtml);
        });
}