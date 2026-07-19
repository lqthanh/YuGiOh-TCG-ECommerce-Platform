import { useContext } from 'react'

import { AppData } from '../../Root';
import Pagination from '../Shared/Pagination';
import usePagedFetch from '../../hooks/usePagedFetch';

import './../../styles/User.css'

export default function UserDealsBoughtSold({ type, apiCall, headerArr, renderDealsRow }) {
    const { userData } = useContext(AppData)

    const {
        items: list,
        currentPage,
        setCurrentPage,
        totalPages,
    } = usePagedFetch(
        (page, pageSize) => apiCall(userData.username, page, pageSize),
        10
    );

    return (
        <div className='user-anything-wrapper'>
            <div className='user-anything-title'>
                <p>
                    <span className='text-primary'>Deals</span>
                    <span className='text-secondary'> {type}</span>
                </p>
            </div>
            <div className='user-anything-list'>
                {list.length ? (
                    <>
                        <div className='user-anything-list-header'>
                            {headerArr.map((col, index) =>
                                <div className={`list-col-${col.key}`} key={index}>
                                    {col.label}
                                </div>
                            )}
                        </div>
                        <div className='line'></div>
                        <div className='user-anything-list-body'>
                            {list.map((deal, dealIndex) => renderDealsRow(deal, dealIndex)).map((row, rowIndex) =>
                            (<div className='list-row' key={rowIndex}>
                                {headerArr.map((item, headerIndex) =>
                                    <div key={headerIndex} className={`list-col-${item.key}`}>
                                        {row[item.data_key]}
                                    </div>
                                )}
                            </div>)
                            )}
                        </div>
                    </>) : <p className='no-data-text'>You haven't {type.toLowerCase()} any cards yet!</p>
                }
            </div>
            <div className='user-anything-footer'>
                <Pagination currentPage={currentPage} totalPages={totalPages} setCurrentPage={setCurrentPage} />
            </div>
        </div>
    )
}
