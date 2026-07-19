import { useContext, useEffect, useState } from "react";

import { AppData } from "../../Root";
import { createDeal } from "../../api/apiDeal";
import {
  getOwnedCardsSeperate,
  getOwnedCardsStack,
} from "../../api/apiUserCard";
import { useNavigate } from 'react-router-dom'
import { checkSession } from "../../utils/checkSession";
import usePagedFetch from "../../hooks/usePagedFetch";

import SearchAllCards from "../Shared/SearchSelections/SearchAllCards";
import Pagination from "../Shared/Pagination";
import ReusableCard from "../Shared/ReusableCard";
import CardDetails from "../Shared/CardDetails";
import FormModal from "../Shared/FormModal";
import Input from "../Shared/Input/Input";

import "./../../styles/UserAllCards.css";

export default function UserAllCards() {
  const { userData, setType, setMessage, showToast } = useContext(AppData);
  const navigate = useNavigate()

  const [isStack, setIsStack] = useState(false);
  const [searchObject, setSeachObject] = useState({
    cardName: "",
    cardTypeName: "",
    cardOriginName: "",
    cardElementName: "",
    cardRarityName: "",
  });
  const [isOpen, setIsOpen] = useState(false);
  const [isAddingDeal, setIsAddingDeal] = useState(false);
  const [cardSelected, setCardSelected] = useState();
  const [dealPrice, setDealPrice] = useState(0);

  const {
    items: cards,
    currentPage,
    setCurrentPage,
    totalPages,
    refresh,
    resetToFirstPage,
  } = usePagedFetch(
    (page, pageSize) => (isStack ? getOwnedCardsStack : getOwnedCardsSeperate)(userData.username, searchObject, page, pageSize),
    15
  );

  const handleToggleViewOption = () => {
    setIsStack(!isStack);
    resetToFirstPage();
  };

  const handleOpenCardDetail = (card) => {
    setCardSelected(card);
    setIsOpen(true);
  };

  const handleAddDeal = () => {
    setIsAddingDeal(true)
  };

  const handleCreateDeal = async () => {
    if (parseInt(dealPrice)) {
      const response = await createDeal(userData.username, cardSelected.userCardId, parseInt(dealPrice))
      response.json().then(data => {
        if (response.status === 200) {
          setType('toast-success')
          setIsAddingDeal(false)
          setIsOpen(false);
          refresh();
        } else {
          setType('toast-error')
        }
        setMessage(data.message);
        showToast();
      })
    }
  }

  useEffect(() => {
    if (!checkSession()) {
      navigate('/')
    }
  })

  return (
    <div className="user-allcards-screen">
      <div className="user-anything-wrapper">
        <div className="user-anything-title">
          <div className="cards-management-wrapper">
            <p>
              <span className="text-primary">Cards</span>{" "}
              <span className="text-secondary"> Management</span>
            </p>
            <div
              className="user-cards-view-option"
              onClick={handleToggleViewOption}
            >
              <div
                className={`${!isStack ? "cards-stack" : "cards-seperate"
                  } icon-8`}
              ></div>
              {!isStack ? (
                <span className="text-third">Stack</span>
              ) : (
                <span className="text-third">Seperate</span>
              )}
            </div>
          </div>
          <SearchAllCards
            searchObject={searchObject}
            setData={setSeachObject}
            onSearch={resetToFirstPage}
          />
        </div>
        <div className="user-anything-container">
          {cards.length ? cards.map((card, index) => (
            <ReusableCard
              key={index}
              card={card}
              onClick={() => handleOpenCardDetail(card)}
            />
          )) :
            <p className="no-data-text">There is no result matching with your search options :(</p>
          }
        </div>
        <Pagination
          currentPage={currentPage}
          totalPages={totalPages}
          setCurrentPage={setCurrentPage}
        />
      </div>
      <FormModal
        title={
          <span className="text-secondary">
            Create a <span className="text-primary">Deal</span>
          </span>
        }
        isDisplay={isAddingDeal}
        inputs={[{
          id: 1,
          label: "Enter price for this deal",
          type: 'number',
        }]}
        renderInput={(input) => <Input label={input.label} type={input.type} setData={(price) => setDealPrice(price)} key={input.id} />}
        setIsDisplay={setIsAddingDeal}
        onSubmit={handleCreateDeal}
      />
      <CardDetails
        isOpen={isOpen}
        selectedCard={cardSelected}
        onClose={() => setIsOpen(false)}
        isManaging
        onAddDeal={handleAddDeal}
      />
    </div>
  );
}
