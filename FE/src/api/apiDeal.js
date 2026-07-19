import API_ROUTES from "../constants/apiRoutes";
import { HEADER } from "../constants/apiHeaderConfig";

const API_URL = import.meta.env.VITE_API_URL;

export const searchDeal = async (filters = {}, page = 1, pageSize = 20) => {
  const {
    myUsername = "",
    sellUsername = "",
    cardName = "",
    cardTypeName = "",
    cardOriginName = "",
    cardElementName = "",
    cardRarityName = "",
    priceFrom = "",
    priceTo = "",
    dateFrom = "",
    dateTo = "",
    sortBy = "",
    isAsc = false,
  } = filters;
  const response = await fetch(
    `${API_URL}${API_ROUTES.SEARCH_DEAL}?MyUsername=${myUsername}&SellUsername=${sellUsername}&CardName=${cardName}&CardTypeName=${cardTypeName}&CardOriginName=${cardOriginName}&CardElementName=${cardElementName}&CardRarityName=${cardRarityName}&PriceFrom=${priceFrom}&PriceTo=${priceTo}&DateFrom=${dateFrom}&DateTo=${dateTo}&sort=${sortBy}&sortAscending=${!!isAsc}&page=${page}&pageSize=${pageSize}`
  );
  return response.json();
};

export const getBoughtDeals = async (username, page = 1, pageSize = 20) => {
  const response = await fetch(
    `${API_URL}${API_ROUTES.GET_BOUGHT_DEAL}?Username=${username}&page=${page}&pageSize=${pageSize}`, {
      method: 'GET',
      headers: HEADER()
    }
  );
  return response.json();
};

export const getSoldDeals = async (username, page = 1, pageSize = 20) => {
  const response = await fetch(
    `${API_URL}${API_ROUTES.GET_SOLD_DEAL}?Username=${username}&page=${page}&pageSize=${pageSize}`, {
      method: 'GET',
      headers: HEADER()
    }
  );
  return response.json();
};

export const createDeal = async (sellUsername, userCardId, price) => {
  console.log('call api');
  const response = fetch(`${API_URL}${API_ROUTES.CREATE_DEAL}`, {
    method: "POST",
    headers: HEADER(),
    body: JSON.stringify({
      sellUsername: sellUsername,
      userCardId: userCardId,
      price: price,
    }),
  });
  return response;
};

export const editDeal = async (sellUsername, dealId, userCardId, price) => {
  const response = await fetch(`${API_URL}${API_ROUTES.EDIT_DEAL}`, {
      method: "PUT",
      headers: HEADER(),
      body: JSON.stringify({
        sellUsername: sellUsername,
        dealId: dealId,
        userCardId: userCardId,
        price: price,
      }),
    });
  return response;
};

export const deleteDeal = async (sellUsername = "", dealId) => {
  const response = fetch(`${API_URL}${API_ROUTES.DELETE_DEAL}`, {
    method: "DELETE",
    headers: HEADER(),
    body: JSON.stringify({
      sellUsername: sellUsername,
      dealId: dealId,
    }),
  });
  return response;
};

export const acceptDeal = async (buyUsername, dealId) => {
  const response = fetch(`${API_URL}${API_ROUTES.ACCEPT_DEAL}`, {
    method: "POST",
    headers: HEADER(),
    body: JSON.stringify({
      buyUsername: buyUsername,
      dealId: dealId,
    }),
  });
  return response;
};
