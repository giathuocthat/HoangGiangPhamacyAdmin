# Product API Documentation

## Overview
The Product API provides endpoints for managing and retrieving product information, including filtering by category.

## Base URL
```
/api/product
```

## Endpoints

### 1. Get All Products
**Endpoint:** `GET /api/product`

**Description:** Retrieve all products from the database.

**Response:**
```json
[
  {
    "id": 1,
    "name": "Product Name",
    "categoryId": 1,
    "category": {
      "id": 1,
      "name": "Category Name"
    }
  }
]
```

**Status Codes:**
- `200 OK` - Successfully retrieved all products
- `500 Internal Server Error` - An error occurred while retrieving products

---

### 2. Get Product by ID
**Endpoint:** `GET /api/product/{id}`

**Description:** Retrieve a specific product by its ID.

**Parameters:**
- `id` (path, required): Product ID (integer)

**Response:**
```json
{
  "id": 1,
  "name": "Product Name",
  "categoryId": 1,
  "category": {
    "id": 1,
    "name": "Category Name"
  }
}
```

**Status Codes:**
- `200 OK` - Product found
- `400 Bad Request` - Invalid product ID (must be > 0)
- `404 Not Found` - Product with specified ID not found
- `500 Internal Server Error` - An error occurred while retrieving the product

---

### 3. Get Products by Category ?
**Endpoint:** `GET /api/product/category/{categoryId}`

**Description:** Retrieve all products that belong to a specific category.

**Parameters:**
- `categoryId` (path, required): Category ID (integer)

**Example Request:**
```
GET /api/product/category/1
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Product Name 1",
    "categoryId": 1,
    "category": {
      "id": 1,
      "name": "Category Name"
    }
  },
  {
    "id": 2,
    "name": "Product Name 2",
    "categoryId": 1,
    "category": {
      "id": 1,
      "name": "Category Name"
    }
  }
]
```

**Status Codes:**
- `200 OK` - Successfully retrieved products for the category
- `400 Bad Request` - Invalid category ID (must be > 0)
- `500 Internal Server Error` - An error occurred while retrieving products

---

### 4. Search Products by Name
**Endpoint:** `GET /api/product/search?name={name}`

**Description:** Search for products by name (partial match).

**Parameters:**
- `name` (query, required): Product name or partial name to search for

**Example Request:**
```
GET /api/product/search?name=aspirin
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Aspirin 500mg",
    "categoryId": 1,
    "category": {
      "id": 1,
      "name": "Pain Relief"
    }
  }
]
```

**Status Codes:**
- `200 OK` - Search completed successfully
- `400 Bad Request` - Product name cannot be null or empty
- `500 Internal Server Error` - An error occurred while searching products

---

## Error Response Format
All error responses follow this format:

```json
{
  "message": "Error description",
  "error": "Optional detailed error message"
}
```

---

## Usage Examples

### cURL
```bash
# Get all products
curl -X GET "https://localhost:7001/api/product"

# Get products by category ID 1
curl -X GET "https://localhost:7001/api/product/category/1"

# Search for products containing "aspirin"
curl -X GET "https://localhost:7001/api/product/search?name=aspirin"
```

### JavaScript/Fetch
```javascript
// Get products by category
async function getProductsByCategory(categoryId) {
  const response = await fetch(`/api/product/category/${categoryId}`);
  const products = await response.json();
  console.log(products);
}

// Search products
async function searchProducts(name) {
  const response = await fetch(`/api/product/search?name=${name}`);
  const products = await response.json();
  console.log(products);
}
```

---

## Implementation Details

### Files Created/Modified:
1. **ProductController.cs** (New) - API controller with all product endpoints
2. **Program.cs** (Modified) - Added dependency injection configuration
3. **ThuocGiaThatAdmin.Server.csproj** (Modified) - Added project references

### Dependencies:
- ProductService (Business Logic)
- IProductRepository (Data Access)
- EntityFrameworkCore (Database)

### Configuration:
The API uses the following connection string (from appsettings.json or default):
```
Server=(localdb)\mssqllocaldb;Database=ThuocGiaThatDb;Trusted_Connection=true;
```

Update this in your `appsettings.json` if needed.
