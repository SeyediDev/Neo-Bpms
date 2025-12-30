# Neo.UI.EditableGrid

Google Sheets-like editable grid component as a React micro-frontend for the Neo BPMS admin panel.

## Features

- ✅ **Google Sheets-like Editing**: Click any cell to edit, just like Google Sheets
- ✅ **Optimized Communication**: Queue management with batching and debouncing
- ✅ **Resilient API Calls**: Circuit breaker pattern and retry logic
- ✅ **Visual Feedback**: 
  - Yellow border before save (pending)
  - Blue background during save
  - Green border flash after successful save
  - Red border for errors
- ✅ **Multiple Data Types**: Text, Number, Date, Boolean, Select, Multi-select
- ✅ **RTL Support**: Full right-to-left support for Persian/Arabic
- ✅ **Jalali Date Support**: Persian calendar integration

## Architecture

### Components

- **EditableGrid**: Main grid component
- **CellEditor**: Cell editor with type-specific controls
- **QueueManager**: Manages change queue with batching
- **CircuitBreaker**: Prevents cascading failures
- **GridApiClient**: API client with retry logic

### Communication Flow

```
User edits cell
    ↓
Local state updated immediately (optimistic update)
    ↓
Change added to queue
    ↓
Queue batches changes (500ms debounce or 10 items)
    ↓
Batch sent to API via CircuitBreaker
    ↓
Visual feedback (yellow → blue → green)
```

## Usage

### In MVC Admin Panel

```csharp
// Controller
public IActionResult Index(string? endpoint = "sample")
{
    ViewBag.Endpoint = endpoint;
    return View();
}
```

```html
<!-- View -->
<iframe 
    src="/editable-grid/?endpoint=@ViewBag.Endpoint"
    style="width: 100%; height: 600px; border: none;">
</iframe>
```

### Configuration

The grid accepts columns configuration:

```typescript
const columns: GridColumn[] = [
  { 
    id: 'name', 
    name: 'نام', 
    type: 'text', 
    editable: true, 
    required: true,
    width: 200 
  },
  { 
    id: 'status', 
    name: 'وضعیت', 
    type: 'select', 
    editable: true,
    options: [
      { value: 'active', label: 'فعال' },
      { value: 'inactive', label: 'غیرفعال' },
    ]
  },
];
```

## API Endpoints

### GET /api/grid/{endpoint}
Get grid data with pagination, sorting, and filtering.

**Query Parameters:**
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 100)
- `sortBy`: Column to sort by
- `sortDirection`: "asc" or "desc"
- `filter`: Filter expression

**Response:**
```json
{
  "columns": [...],
  "rows": [...],
  "totalCount": 100,
  "page": 1,
  "pageSize": 100
}
```

### POST /api/grid/{endpoint}/batch-update
Batch update multiple cells.

**Request:**
```json
{
  "changes": [
    {
      "rowId": "123",
      "columnId": "name",
      "value": "New Name"
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "updated": 1,
  "errors": []
}
```

### GET /api/grid/{endpoint}/config
Get grid configuration (columns, types, etc.).

## Development

```bash
# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Export static files
npm run export
```

## Integration

1. Build the micro-frontend:
   ```bash
   cd src/Neo.UI.EditableGrid
   npm run export
   ```

2. Copy `dist` folder to MVC admin panel `wwwroot/editable-grid`

3. Register API endpoints in `Program.cs`:
   ```csharp
   services.AddNeoBpmsApi(configuration);
   ```

4. Map endpoints:
   ```csharp
   app.MapNeoBpmsApiEndpoints();
   ```

## Queue Configuration

Default queue settings:
- **Batch Size**: 10 changes
- **Batch Delay**: 500ms (debounce)
- **Max Retries**: 3
- **Retry Delay**: 1000ms (with exponential backoff)

These can be customized when creating the QueueManager.

## Circuit Breaker Configuration

Default circuit breaker settings:
- **Failure Threshold**: 5 failures
- **Reset Timeout**: 30 seconds
- **Monitoring Window**: 60 seconds

The circuit opens after 5 failures within 60 seconds and stays open for 30 seconds before attempting to close.

## Future Enhancements

- [ ] Virtual scrolling for large datasets
- [ ] Column resizing and reordering
- [ ] Row selection and bulk operations
- [ ] Copy/paste support
- [ ] Undo/redo functionality
- [ ] Formula support
- [ ] Cell formatting
- [ ] Export to Excel/CSV

