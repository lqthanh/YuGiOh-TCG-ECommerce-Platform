import API_ROUTES from "../constants/apiRoutes";

const API_URL = import.meta.env.VITE_API_URL;

export const searchCard = async (filters = {}, page = 1, pageSize = 20) => {
  const {
    cardName = '',
    cardTypeName = '',
    cardOriginName = '',
    cardElementName = '',
    cardRarityName = '',
  } = filters;
  const url = `${API_URL}${API_ROUTES.SEARCH_CARD}?CardName=${cardName}&CardTypeName=${cardTypeName}&CardOriginName=${cardOriginName}&CardElementName=${cardElementName}&CardRarityName=${cardRarityName}&page=${page}&pageSize=${pageSize}`;
  try {
    const response = await fetch(url);
    return response.json();
  } catch (error) {
    console.error(error);
  }
};


//something la 1 route ben trong API_ROUTES
export const getSomeThingOfCard = async (someThing) => {
    const url = `${API_URL}${someThing}`;
    try {
        const response = await fetch(url);
        return response.json();
    } catch(error) {
        console.log(error);
    }
}


