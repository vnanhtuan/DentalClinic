// Import hằng số pagination
import { PAGE_SIZE_OPTIONS, DEFAULT_PAGE_SIZE } from '../constants/paginationConstants.js';

export const PaginationComponent = {
    template: `
        <div v-if="totalPages > 0" class="v-data-table-footer mt-4 ga-3">
            <div class="d-flex align-center ga-2">
                <span class="text-body-2">Hiển thị:</span>
                <v-select 
                    :model-value="pageSize" 
                    :items="pageSizeOptions"
                    variant="outlined" 
                    density="compact" 
                    hide-details 
                    style="max-width: 100px;" 
                    @update:model-value="onPageSizeChange"></v-select>
                <span class="text-body-2">Kết quả: {{ fromItem }} - {{ toItem }} / {{ totalItems }}</span>
            </div>
            <v-pagination 
                :model-value="currentPage"
                @update:model-value="$emit('update:currentPage', $event)"
                :length="totalPages" 
                :total-visible="5"
                density="comfortable">
            </v-pagination>
        </div>
    `,
    props: {
        // Sử dụng v-model:currentPage
        currentPage: {
            type: Number,
            required: true
        },
        // Sử dụng v-model:pageSize
        pageSize: {
            type: Number,
            default: DEFAULT_PAGE_SIZE
        },
        // Tổng số item từ server
        totalItems: {
            type: Number,
            required: true
        },
        // Mảng options cho v-select
        pageSizeOptions: {
            type: Array,
            default: () => PAGE_SIZE_OPTIONS
        }
    },
    // Khai báo các sự kiện component này sẽ phát ra
    emits: [
        'update:currentPage', // Cho v-model:currentPage
        'update:pageSize',    // Cho v-model:pageSize
        'pageSizeChanged'     // Thông báo cho parent rằng pageSize đã thay đổi (để gọi API)
    ],
    computed: {
        // Tính toán nội bộ
        totalPages() {
            if (!this.totalItems || !this.pageSize) return 0;
            return Math.ceil(this.totalItems / this.pageSize);
        },
        fromItem() {
            if (this.totalItems === 0) return 0;
            return (this.currentPage - 1) * this.pageSize + 1;
        },
        toItem() {
            const end = this.currentPage * this.pageSize;
            return end > this.totalItems ? this.totalItems : end;
        }
    },
    methods: {
        // Khi người dùng thay đổi v-select page size
        onPageSizeChange(newSize) {
            // 1. Cập nhật v-model:pageSize của parent
            this.$emit('update:pageSize', newSize);
            // 2. Phát sự kiện để parent biết và gọi lại API (triggerFilterChange)
            this.$emit('pageSizeChanged');
        }
    }
};