document.addEventListener('DOMContentLoaded', () => {

    // Function to re-bind event listeners after DOM replacement
    const bindDynamicListeners = () => {
        // --- Add to Cart Logic ---
        const addToCartButtons = document.querySelectorAll('.add-to-cart-btn');
        addToCartButtons.forEach(button => {
            // Remove old listener to prevent duplicates if function runs twice
            const newButton = button.cloneNode(true);
            button.parentNode.replaceChild(newButton, button);

            newButton.addEventListener('click', async (e) => {
                e.preventDefault();
                e.stopPropagation();

                const productId = newButton.getAttribute('data-id');
                const originalText = newButton.innerHTML;

                newButton.innerHTML = '<i class="bi bi-hourglass-split"></i> Adding...';
                newButton.disabled = true;

                try {
                    const response = await fetch(`/ShoppingCart/AddItemJson?id=${productId}`, { method: 'POST' });
                    const text = await response.text();

                    if (!response.ok) {
                        toastr.error(`Server rejected the request (Error ${response.status}).`, 'Error');
                        newButton.innerHTML = originalText;
                        newButton.disabled = false;
                        return;
                    }

                    const data = JSON.parse(text);
                    if (data.success) {
                        newButton.innerHTML = '<i class="bi bi-check-circle-fill"></i> Added!';
                        newButton.classList.replace('bg-blue-600', 'bg-emerald-600');
                        newButton.classList.replace('hover:bg-blue-500', 'hover:bg-emerald-500');

                        const badge = document.getElementById('cart-badge-count');
                        if (badge) {
                            badge.innerText = data.cartCount;
                            badge.classList.remove('hidden');
                        }

                        setTimeout(() => {
                            newButton.innerHTML = originalText;
                            newButton.classList.replace('bg-emerald-600', 'bg-blue-600');
                            newButton.classList.replace('hover:bg-emerald-500', 'hover:bg-blue-500');
                            newButton.disabled = false;
                        }, 2000);
                    } else {
                        newButton.innerHTML = '<i class="bi bi-x-circle-fill"></i> Unavailable';
                        newButton.classList.replace('bg-blue-600', 'bg-red-600');
                        newButton.classList.replace('hover:bg-blue-500', 'hover:bg-red-500');
                        if (data.message) toastr.warning(data.message, 'Notice');

                        setTimeout(() => {
                            newButton.innerHTML = originalText;
                            newButton.classList.replace('bg-red-600', 'bg-blue-600');
                            newButton.classList.replace('hover:bg-red-500', 'hover:bg-blue-500');
                            newButton.disabled = false;
                        }, 2000);
                    }
                } catch (error) {
                    toastr.error("A network error occurred. Check the console.", 'Error');
                    newButton.innerHTML = originalText;
                    newButton.disabled = false;
                }
            });
        });

        // --- Buy Now Logic ---
        const buyNowButtons = document.querySelectorAll('.buy-now-btn');
        buyNowButtons.forEach(button => {
            const newButton = button.cloneNode(true);
            button.parentNode.replaceChild(newButton, button);

            newButton.addEventListener('click', async (e) => {
                e.preventDefault();
                e.stopPropagation();

                const productId = newButton.getAttribute('data-id');
                const originalText = newButton.innerHTML;
                newButton.innerHTML = '<i class="bi bi-hourglass-split"></i> Redirecting...';
                newButton.disabled = true;

                try {
                    const response = await fetch(`/ShoppingCart/AddItemJson?id=${productId}`, { method: 'POST' });
                    if (response.ok) {
                        const data = await response.json();
                        if (data.success) {
                            window.location.href = '/ShoppingCart/ShoppingCart';
                        } else {
                            toastr.warning(data.message || 'Item unavailable', 'Notice');
                            newButton.innerHTML = originalText;
                            newButton.disabled = false;
                        }
                    } else {
                        toastr.error('Server error occurred.', 'Error');
                        newButton.innerHTML = originalText;
                        newButton.disabled = false;
                    }
                } catch (error) {
                    toastr.error("Network error.", 'Error');
                    newButton.innerHTML = originalText;
                    newButton.disabled = false;
                }
            });
        });

        // --- Wishlist Toggle Logic ---
        const wishlistButtons = document.querySelectorAll('.wishlist-btn');
        wishlistButtons.forEach(button => {
            const newButton = button.cloneNode(true);
            button.parentNode.replaceChild(newButton, button);

            newButton.addEventListener('click', async (e) => {
                e.preventDefault();
                e.stopPropagation();

                const productId = newButton.getAttribute('data-id');
                const icon = newButton.querySelector('i');
                newButton.disabled = true;

                try {
                    const response = await fetch(`/Wishlist/Toggle?productId=${productId}`, { method: 'POST' });
                    if (response.ok) {
                        const data = await response.json();
                        if (data.success) {
                            if (data.isAdded) {
                                icon.classList.remove('bi-heart');
                                icon.classList.add('bi-heart-fill', 'text-pink-500');
                                toastr.success('Added to your wishlist!', 'Saved');
                            } else {
                                icon.classList.remove('bi-heart-fill', 'text-pink-500');
                                icon.classList.add('bi-heart');
                                toastr.info('Removed from your wishlist.', 'Updated');

                                const cardElement = document.getElementById(`wishlist-card-${productId}`);
                                if (cardElement) {
                                    cardElement.classList.add('opacity-0', 'scale-95');
                                    setTimeout(() => {
                                        cardElement.remove();
                                        const remainingCards = document.querySelectorAll('[id^="wishlist-card-"]');
                                        if (remainingCards.length === 0) window.location.reload();
                                    }, 300);
                                }
                            }
                        } else {
                            toastr.error(data.message || 'Could not update wishlist.', 'Oops!');
                        }
                    } else {
                        if (response.status === 401) window.location.href = '/Identity/Account/Login';
                        else toastr.error('Server error occurred.', 'Error');
                    }
                } catch (error) {
                    toastr.error("Network error.", 'Error');
                } finally {
                    newButton.disabled = false;
                }
            });
        });
    };

    // Run it on initial load
    bindDynamicListeners();

    // Async Dynamic Filtering & Pagination

    const filterForm = document.getElementById('filter-form');
    const gridContainer = document.getElementById('product-grid-container');

    if (filterForm && gridContainer) {

        const fetchResults = async (urlToFetch = null) => {
            let url = urlToFetch;
            if (!url) {
                // Manually build URL from inputs
                const params = new URLSearchParams();
                const inputs = filterForm.querySelectorAll('input, select');
                inputs.forEach(i => {
                    if (i.name && i.value) params.append(i.name, i.value);
                });
                const basePath = filterForm.getAttribute('action') || window.location.pathname;
                url = `${basePath}?${params.toString()}`;
            }

            try {
                const response = await fetch(url);
                if (!response.ok) throw new Error('Network response failed');
                const htmlText = await response.text();

                const parser = new DOMParser();
                const virtualDoc = parser.parseFromString(htmlText, 'text/html');
                const newGrid = virtualDoc.getElementById('product-grid-container');

                if (newGrid) {
                    gridContainer.innerHTML = newGrid.innerHTML;
                    window.history.pushState({}, '', url);

                    // Critical: Re-bind all cart/wishlist buttons inside the HTML!
                    bindDynamicListeners();
                }
            } catch (error) {
                console.error("Async filtering failed:", error);
                filterForm.submit();
            }
        };

        const filterInputs = filterForm.querySelectorAll('input, select');
        let debounceTimer;

        filterInputs.forEach(input => {
            if (input.type === 'text' || input.type === 'search') {
                input.addEventListener('input', () => { // Changed keyup to input for better mobile support
                    clearTimeout(debounceTimer);
                    debounceTimer = setTimeout(() => fetchResults(), 500);
                });
            } else {
                input.addEventListener('change', () => fetchResults());
            }
        });

        // Intercept Pagination Clicks
        gridContainer.addEventListener('click', (e) => {
            const link = e.target.closest('a');
            if (link && link.href && !link.classList.contains('pointer-events-none') && !link.href.includes('Details')) {
                e.preventDefault();
                gridContainer.style.opacity = '0.5';
                fetchResults(link.href).then(() => {
                    gridContainer.style.opacity = '1';
                    filterForm.scrollIntoView({ behavior: 'smooth', block: 'start' });
                });
            }
        });
    }

});

// Visibility Toggle Function (AJAX)
function toggleProductVisibility(e, productId, btnElement) {
    e.preventDefault();
    e.stopPropagation();

    const button = $(btnElement);
    const badge = button.find('.status-badge');
    const originalHtml = badge.html();

    const removeOnHide = button.attr('data-remove-on-hide') === 'true';
    const card = button.closest('.group');

    badge.html('<i class="bi bi-arrow-repeat animate-spin"></i>');

    $.post('/Product/ToggleVisibility', { id: productId })
        .done(function (data) {
            if (data.success) {
                if (data.isVisible) {
                    badge.removeClass('bg-slate-600/50 text-slate-400 border-slate-600')
                        .addClass('bg-emerald-500/20 text-emerald-400 border-emerald-500/30')
                        .text('Visible');
                } else {
                    if (removeOnHide) {
                        card.fadeOut(300, function () {
                            $(this).remove();
                        });
                    } else {
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