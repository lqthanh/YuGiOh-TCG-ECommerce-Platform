import API_ROUTES from "../constants/apiRoutes";
import { HEADER } from "../constants/apiHeaderConfig";

const API_URL = import.meta.env.VITE_API_URL;

const buildOwnedCardsUrl = (route, username, filters, page, pageSize) => {
    const {
        cardName = '',
        cardTypeName = '',
        cardOriginName = '',
        cardElementName = '',
        cardRarityName = '',
    } = filters;
    return `${API_URL}${route}/?Username=${username}&CardName=${cardName}&CardTypeName=${cardTypeName}&CardOriginName=${cardOriginName}&CardElementName=${cardElementName}&CardRarityName=${cardRarityName}&page=${page}&pageSize=${pageSize}`;
}

export const getOwnedCardsSeperate = async (username, filters = {}, page = 1, pageSize = 20) => {
    const response = await fetch(buildOwnedCardsUrl(API_ROUTES.USERCARD_SEARCH_OWNED_SEPARATE, username, filters, page, pageSize), {
        method: 'GET',
        headers: HEADER(),
    });
    return response.json();
}

export const getOwnedCardsStack = async (username, filters = {}, page = 1, pageSize = 20) => {
    const response = await fetch(buildOwnedCardsUrl(API_ROUTES.USERCARD_SEARCH_OWNED_STACK, username, filters, page, pageSize), {
        method: 'GET',
        headers: HEADER(),
    });
    return response.json();
}
