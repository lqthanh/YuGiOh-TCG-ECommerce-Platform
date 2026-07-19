/* eslint-disable react/prop-types */
import { useState } from "react";

import { searchCard } from "../../api/apiCard";
import usePagedFetch from "../../hooks/usePagedFetch";

import CardDetails from "../Shared/CardDetails";
import Pagination from "../Shared/Pagination";
import SearchAllCards from "../Shared/SearchSelections/SearchAllCards";

import "./../../styles/CardDetails.css";
import "./../../styles/Body.css";

function Body() {
  const [isCardDetailsOpen, setCardDetailsOpen] = useState(false);
  const [selectedCard, setSelectedCard] = useState(null);
  const [searchObject, setSearchObject] = useState({
    cardName: "",
    cardTypeName: "",
    cardOriginName: "",
    cardElementName: "",
    cardRarityName: "",
  });

  const {
    items: cards,
    currentPage,
    setCurrentPage,
    totalPages,
    resetToFirstPage,
  } = usePagedFetch(
    (page, pageSize) => searchCard(searchObject, page, pageSize),
    10
  );

  const openDetails = (cards) => {
    setSelectedCard(cards);
    setCardDetailsOpen(true);
  };

  const closeDetails = () => {
    setSelectedCard(null);
    setCardDetailsOpen(false);
  };

  return (
    <>
      <div className="body-session">
        <div className="body-container-wrapper">
          <div className="all-cards-header">
            <div className="all-cards-header-title">
              <span className="text-secondary">Avaiable</span>
              <span className="text-primary"> Cards</span>
            </div>
            <SearchAllCards
              searchObject={searchObject}
              setData={setSearchObject}
              onSearch={resetToFirstPage}
            />
          </div>
          <div className="body-container">
            {cards.length ? (
              cards.map((item, index) => (
                <div
                  className="cards"
                  key={index}
                  onClick={() => openDetails(item)}
                >
                  <div className={`rarity ${item.cardRarityName}`}>
                    {item.cardRarityName}
                  </div>
                  <img src={item.cardImageURL} alt="" className="cards-img" />
                </div>
              ))
            ) : (
              <p className="not-found text-secondary">
                Sorry, we couldn't find what you want :(
              </p>
            )}
          </div>
          {<Pagination currentPage={currentPage} totalPages={totalPages} setCurrentPage={setCurrentPage} />}
        </div>
      </div>

      <CardDetails
        isOpen={isCardDetailsOpen}
        selectedCard={selectedCard}
        onClose={closeDetails}
      />
    </>
  );
}

export default Body;
