import AdSlider from "../AdSlider/AdSlider"
import AllDealsBody from "./AllDealsBody"

function AllDealsClientView() {

    return (
        <>
            <div className="AllDeals-main-content">
                <AdSlider />
                <AllDealsBody />
            </div>
        </>
    )
}

export default AllDealsClientView;
