import { NextPage } from "next";
import { Metadata } from "../components/core/metadata.component";
import { Home } from "../components/home/home.component";

const HomePage: NextPage = () => {
    return (
        <>
            <Metadata />
            <Home />
        </>
    );
};

export default HomePage;
