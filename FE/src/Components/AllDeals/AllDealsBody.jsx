import { useState, useEffect, useContext } from "react";

import { acceptDeal, searchDeal } from '../../api/apiDeal'
import { getMoney } from "../../api/apiUser";
import dateRangeConfig from './../../utils/dateRangeConfig'
import priceRangeConfig from "../../utils/priceRangeConfig";
import usePagedFetch from "../../hooks/usePagedFetch";

import { AppData } from "../../Root";
import DealDetails from "../Shared/DealDetail";
import Pagination from "../Shared/Pagination";
import ConfirmModal from "../Shared/ConfirmModal";
import SearchAllDeals from "../Shared/SearchSelections/SearchAllDeals";

import '../../styles/AllDeals.css'

function AllDealsBody() {

    const { userData, setUserData, setType, setMessage, showToast } = useContext(AppData);

    const [selectedDeal, setSelectedDeal] = useState();
    const [isDealDetailsOpen, setDealDetailsOpen] = useState(false);
    const [searchObject, setSearchObject] = useState({
        sellUsername: '',
        cardName: "",
        cardTypeName: "",
        cardOriginName: "",
        cardElementName: "",
        cardRarityName: "",
        priceFromTo: '',
        dateFromTo: '',
        sortBy: '',
        isAsc: false,
    });
    const [isConfirmOpen, setConfirmOpen] = useState(false)

    const {
        items: deals,
        currentPage,
        setCurrentPage,
        totalPages,
        refresh,
        resetToFirstPage,
    } = usePagedFetch(
        (page, pageSize) => searchDeal({
            ...searchObject,
            myUsername: userData.username,
            priceFrom: priceRangeConfig(searchObject.priceFromTo).valueFrom,
            priceTo: priceRangeConfig(searchObject.priceFromTo).valueTo,
            dateFrom: dateRangeConfig(searchObject.dateFromTo).valueFrom,
            dateTo: dateRangeConfig(searchObject.dateFromTo).valueTo,
        }, page, pageSize),
        15
    );

    const openDealDetails = (deals) => {
        setSelectedDeal(deals);
        setDealDetailsOpen(true);
    };

    const closeDealDetails = () => {
        setSelectedDeal(null);
        setDealDetailsOpen(false);
    };

    const handleAcceptDeal = async () => {
        const response = await acceptDeal(userData.username, selectedDeal.dealId);
        if (response.status !== 401) {
            response.json().then(data => {
                if (response.status === 200) {
                    setType('toast-success');
                    setDealDetailsOpen(false);
                    getMoney(userData.username).then(money => {
                        setUserData(prev => ({
                            ...prev,
                            money: money
                        }))
                    })
                    refresh();
                } else {
                    setType('toast-error');
                }
                setMessage(data.message);
            })
        } else {
            setType('toast-error');
            setMessage('Please login to purchase this deal!')
        }
        showToast();
    }

    function handleConfirmOpen(deal) {
        if (!selectedDeal) {
            setSelectedDeal(deal)
        }
        setConfirmOpen(true)
    }

    useEffect(() => {
        if (!isConfirmOpen && !isDealDetailsOpen) {
            setSelectedDeal(null)
        }
    }, [isConfirmOpen])

    return (
        <>
            <div className="AllDeals-body">
                <div className="AllDeals-body-container-wrapper">
                    <div className="all-deals-header">
                        <div className="all-deals-header-text">
                            <span className="text-secondary">Avaiable</span>
                            <span className="text-primary"> Deals</span>
                        </div>
                        <SearchAllDeals searchObject={searchObject} setData={setSearchObject} onSearch={resetToFirstPage} />
                    </div>
                    <div className="AllDeals-body-container">
                        {
                            deals.length ? deals.map((item, index) =>
                                <div className='AllDeals-deals' key={index}>
                                    <div className="AllDeals-cards" onClick={() => openDealDetails(item)}>
                                        <div className={`rarity ${item.cardRarityName}`}>
                                            {item.cardRarityName}
                                        </div>
                                        <img src={item.cardImageURL} alt="" className='AllDeals-deals-img' />
                                    </div>
                                    <div className="AllDeals-bottom">
                                        <div className="AllDeals-price">
                                            <div className="riu-coin-icon icon-9"></div>
                                            {item.price}
                                        </div>
                                        <button className="AllDeals-buy" onClick={() => handleConfirmOpen(item)}>Buy</button>
                                    </div>
                                </div>
                            ) : <p className='not-found text-secondary'>
                                Sorry, we couldn't find what you want :(
                            </p>
                        }
                    </div>
                    {<Pagination currentPage={currentPage} totalPages={totalPages} setCurrentPage={setCurrentPage} />}
                </div>
            </div>
            <DealDetails isOpen={isDealDetailsOpen} selectedDeal={selectedDeal} onClose={closeDealDetails} onBuy={() => handleConfirmOpen(selectedDeal)} />
            <ConfirmModal isOpen={isConfirmOpen} title={<span>Purchase Deal</span>} content={<span>Are you sure you want to purchase this deal?</span>} setIsOpen={setConfirmOpen} onOK={handleAcceptDeal} />
        </>
    )
}

export default AllDealsBody
