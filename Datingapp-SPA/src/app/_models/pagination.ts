export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PagenatedResult<T> {
    result: T;
    pagination: Pagination;
}
