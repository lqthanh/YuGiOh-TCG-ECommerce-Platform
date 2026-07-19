import { useCallback, useEffect, useRef, useState } from "react";

// Giu state phan trang cho cac API tra ve { items, page, pageSize, totalCount, totalPages }.
// fetcher nhan (page, pageSize) va luon duoc goi o ban moi nhat, nen component
// khong can boc no trong useCallback.
export default function usePagedFetch(fetcher, pageSize) {
    const [items, setItems] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);
    const [totalCount, setTotalCount] = useState(0);
    const [reloadKey, setReloadKey] = useState(0);

    const fetcherRef = useRef(fetcher);
    fetcherRef.current = fetcher;

    useEffect(() => {
        let ignore = false;
        fetcherRef.current(currentPage, pageSize).then((data) => {
            if (ignore || !data) return;
            setItems(data.items ?? []);
            setTotalPages(data.totalPages ?? 0);
            setTotalCount(data.totalCount ?? 0);
        });
        return () => {
            ignore = true;
        };
    }, [currentPage, pageSize, reloadKey]);

    // Trang hien tai co the vuot qua so trang con lai sau khi du lieu bi xoa bot.
    useEffect(() => {
        if (totalPages > 0 && currentPage > totalPages) {
            setCurrentPage(totalPages);
        }
    }, [currentPage, totalPages]);

    const refresh = useCallback(() => setReloadKey((key) => key + 1), []);

    const resetToFirstPage = useCallback(() => {
        setCurrentPage(1);
        setReloadKey((key) => key + 1);
    }, []);

    return {
        items,
        currentPage,
        setCurrentPage,
        totalPages,
        totalCount,
        refresh,
        resetToFirstPage,
    };
}
