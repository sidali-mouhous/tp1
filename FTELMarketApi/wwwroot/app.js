const productForm = document.getElementById('productForm');
const categoryForm = document.getElementById('categoryForm');
const productIdField = document.getElementById('productId');
const formTitle = document.getElementById('formTitle');
const cancelEditButton = document.getElementById('cancelEdit');
const categorySelect = document.getElementById('categoryId');
const productsContainer = document.getElementById('products');
const categoriesContainer = document.getElementById('categories');
const statsContainer = document.getElementById('stats');

let categories = [];
let products = [];
let activeCategoryFilter = null;

async function loadCategories() {
  const response = await fetch('/api/categories');
  if (!response.ok) throw new Error('Erreur chargement catégories');
  categories = await response.json();
  renderCategorySelector();
  renderCategories();
}

async function loadProducts() {
  const response = await fetch('/api/catalog');
  if (!response.ok) throw new Error('Erreur chargement catalogue');
  products = await response.json();
  renderProducts();
  renderStats();
}

function renderCategorySelector() {
  categorySelect.innerHTML = '';

  categories.forEach((category) => {
    const option = document.createElement('option');
    option.value = category.id;
    option.textContent = category.name;
    categorySelect.appendChild(option);
  });
}

function renderCategories() {
  categoriesContainer.innerHTML = '';

  const allChip = document.createElement('button');
  allChip.type = 'button';
  allChip.className = `category-chip ${activeCategoryFilter === null ? 'active' : ''}`;
  allChip.textContent = 'Tous';
  allChip.addEventListener('click', () => {
    activeCategoryFilter = null;
    renderCategories();
    renderProducts();
  });
  categoriesContainer.appendChild(allChip);

  categories.forEach((category) => {
    const chip = document.createElement('button');
    chip.type = 'button';
    chip.className = `category-chip ${activeCategoryFilter === category.id ? 'active' : ''}`;
    chip.textContent = category.name;
    chip.addEventListener('click', () => {
      activeCategoryFilter = activeCategoryFilter === category.id ? null : category.id;
      renderCategories();
      renderProducts();
    });
    categoriesContainer.appendChild(chip);
  });
}

function renderProducts() {
  const filtered = activeCategoryFilter == null
    ? products
    : products.filter(product => product.categoryId === Number(activeCategoryFilter));

  if (filtered.length === 0) {
    productsContainer.innerHTML = '<p class="meta">Aucun produit pour cette catégorie.</p>';
    return;
  }

  productsContainer.innerHTML = filtered.map((product) => {
    const category = categories.find(cat => cat.id === product.categoryId);
    return `
      <article class="product-card">
        <div class="product-head">
          <div>
            <h4>${escapeHtml(product.name)}</h4>
            <div class="meta">${category ? escapeHtml(category.name) : 'Catégorie inconnue'}</div>
          </div>
          <span class="badge">Stock: ${product.quantityInStock}</span>
        </div>

        <div class="meta">${product.description ? escapeHtml(product.description) : 'Aucune description'}</div>
        <div class="price">${Number(product.price).toFixed(2)} €</div>
        <div class="meta">Créé le ${new Date(product.createdAt).toLocaleDateString('fr-FR')}</div>

        <div class="card-actions">
          <button type="button" class="secondary" data-id="${product.id}" data-action="edit">Modifier</button>
          <button type="button" class="danger" data-id="${product.id}" data-action="delete">Supprimer</button>
        </div>
      </article>
    `;
  }).join('');

  productsContainer.querySelectorAll('[data-action="edit"]').forEach(button => {
    button.addEventListener('click', () => editProduct(Number(button.dataset.id)));
  });

  productsContainer.querySelectorAll('[data-action="delete"]').forEach(button => {
    button.addEventListener('click', () => deleteProduct(Number(button.dataset.id)));
  });
}

function renderStats() {
  const totalProducts = products.length;
  const totalQuantity = products.reduce((sum, product) => sum + product.quantityInStock, 0);
  const avgPrice = totalProducts === 0 ? 0 : products.reduce((sum, product) => sum + Number(product.price), 0) / totalProducts;

  statsContainer.innerHTML = `
    <div class="stat-box">
      <span class="meta">Produits</span>
      <strong>${totalProducts}</strong>
    </div>
    <div class="stat-box">
      <span class="meta">Stock total</span>
      <strong>${totalQuantity}</strong>
    </div>
    <div class="stat-box">
      <span class="meta">Prix moyen</span>
      <strong>${avgPrice.toFixed(2)} €</strong>
    </div>
  `;
}

function escapeHtml(value) {
  return String(value)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#039;');
}

async function createCategory(event) {
  event.preventDefault();
  const input = document.getElementById('categoryName');
  const name = input.value.trim();

  if (!name) return;

  const response = await fetch('/api/categories', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name })
  });

  if (!response.ok) {
    alert('Impossible de créer la catégorie.');
    return;
  }

  input.value = '';
  await loadCategories();
}

async function saveProduct(event) {
  event.preventDefault();

  const payload = {
    name: document.getElementById('name').value.trim(),
    description: document.getElementById('description').value.trim(),
    price: Number(document.getElementById('price').value),
    quantityInStock: Number(document.getElementById('quantityInStock').value),
    categoryId: Number(document.getElementById('categoryId').value)
  };

  const id = productIdField.value;
  const isUpdate = !!id;

  const response = await fetch(isUpdate ? `/api/catalog/${id}` : '/api/catalog', {
    method: isUpdate ? 'PUT' : 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });

  if (!response.ok) {
    const data = await response.text();
    alert(data || 'Erreur lors de l\'enregistrement du produit.');
    return;
  }

  productForm.reset();
  productIdField.value = '';
  formTitle.textContent = 'Ajouter un produit';
  cancelEditButton.classList.add('hidden');
  await loadProducts();
}

async function editProduct(id) {
  const product = products.find(item => item.id === id);
  if (!product) return;

  productIdField.value = product.id;
  document.getElementById('name').value = product.name;
  document.getElementById('description').value = product.description || '';
  document.getElementById('price').value = product.price;
  document.getElementById('quantityInStock').value = product.quantityInStock;
  document.getElementById('categoryId').value = product.categoryId;
  formTitle.textContent = 'Modifier le produit';
  cancelEditButton.classList.remove('hidden');
  window.location.hash = '#add-product';
}

async function deleteProduct(id) {
  if (!window.confirm('Voulez-vous vraiment supprimer ce produit ?')) return;

  const response = await fetch(`/api/catalog/${id}`, { method: 'DELETE' });
  if (!response.ok) {
    alert('Suppression impossible.');
    return;
  }

  await loadProducts();
}

categoryForm.addEventListener('submit', createCategory);
productForm.addEventListener('submit', saveProduct);
cancelEditButton.addEventListener('click', () => {
  productForm.reset();
  productIdField.value = '';
  formTitle.textContent = 'Ajouter un produit';
  cancelEditButton.classList.add('hidden');
});

async function init() {
  try {
    await loadCategories();
    await loadProducts();
  } catch (error) {
    productsContainer.innerHTML = `<p class="meta">${error.message}</p>`;
  }
}

init();
