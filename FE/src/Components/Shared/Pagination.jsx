import "./../../styles/Pagination.css";

export default function Pagination({ currentPage, totalPages, setCurrentPage }) {

    const genarateSibling = () => {
        let result = [];
        if (totalPages > 7) {
            switch (currentPage) {
                case 1:
                    result = [currentPage + 1, currentPage + 2];
                    break;
                case 2:
                    result = [currentPage, currentPage + 1];
                    break;
                case 4:
                    result = [
                        currentPage - 2,
                        currentPage - 1,
                        currentPage,
                        currentPage + 1,
                    ];
                    break;
                case totalPages - 3:
                    result = [
                        currentPage - 1,
                        currentPage,
                        currentPage + 1,
                        currentPage + 2,
                    ];
                    break;
                case totalPages - 1:
                    result = [currentPage - 1, currentPage];
                    break;
                case totalPages:
                    result = [currentPage - 2, currentPage - 1];
                    break;
                default:
                    result = [currentPage - 1, currentPage, currentPage + 1];
                    break;
            }
        } else if (totalPages === 7) {
            if (currentPage <= 2) {
                result = [2, 3];
            } else if (currentPage === 4) {
                result = [
                    currentPage - 2,
                    currentPage - 1,
                    currentPage,
                    currentPage + 1,
                    currentPage + 2,
                ];
            } else if (currentPage > 2 && currentPage < totalPages - 1) {
                result = [currentPage - 1, currentPage, currentPage + 1];
            } else if (currentPage >= totalPages - 1) {
                result = [totalPages - 2, totalPages - 1];
            }
        } else if (totalPages === 6) {
            if (currentPage <= 2) {
                result = [2, 3];
            } else if (currentPage > 2 && currentPage < totalPages - 1) {
                result = [2, 3, 4, 5];
            } else {
                result = [4, 5];
            }
        }
        return result;
    };

    const handlePrevious = () => {
        if (currentPage > 1) {
            setCurrentPage(currentPage - 1);
        }
    };

    const handleNext = () => {
        if (currentPage < totalPages) {
            setCurrentPage(currentPage + 1);
        }
    };

    return (
        <>
            {totalPages > 5 ? (
                <div className="pagination-bar">
                    <span className="nav-btn text-primary" onClick={handlePrevious}>
                        Previous
                    </span>
                    <div className="page-numbers">
                        <span className={`page-btn ${currentPage === 1 && "current-page"}`} onClick={() => setCurrentPage(1)}>
                            1
                        </span>
                        {currentPage > 4 && <span className="page-btn">...</span>}
                        {totalPages > 3 && (
                            <div className="siblings">
                                {genarateSibling().map((pageNumber, index) => (
                                    <span
                                        key={index}
                                        className={`page-btn ${currentPage === pageNumber && "current-page"
                                            }`}
                                        onClick={() => setCurrentPage(pageNumber)}
                                    >
                                        {pageNumber}
                                    </span>
                                ))}
                            </div>
                        )}
                        {currentPage < totalPages - 3 && (
                            <span className="page-btn">...</span>
                        )}
                        <span
                            className={`page-btn ${currentPage === totalPages && "current-page"
                                }`}
                            onClick={() => setCurrentPage(totalPages)}
                        >
                            {totalPages}
                        </span>
                    </div>
                    <span className="nav-btn text-primary" onClick={handleNext}>
                        Next
                    </span>
                </div>
            ) : (<>{totalPages > 0 && <div className="pagination-bar">
                <span className="nav-btn text-primary" onClick={handlePrevious}>
                    Previous
                </span>
                <div className="page-numbers">
                    {Array.from({ length: totalPages }, (_, index) => index + 1).map((item) => (
                        <span
                            className={`page-btn ${currentPage === item && "current-page"}`}
                            key={item}
                            onClick={() => setCurrentPage(item)}
                        >
                            {item}
                        </span>
                    ))}
                </div>
                <span className="nav-btn text-primary" onClick={handleNext}>
                    Next
                </span>
            </div>}</>

            )}
        </>
    );
}
