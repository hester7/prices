import { useQuery } from "@apollo/client";
import { createContext, ReactNode } from "react";
import { PriceIndex, PriceMarket, Query } from "../gql/codegen/graphql";
import enumMetadataQuery from "../gql/documents/queries/enumMetadataQuery";

interface EnumMetadataContextState {
    priceIndexes: Array<PriceIndex>;
    priceMarkets: Array<PriceMarket>;
}

export const EnumMetadataContext = createContext<EnumMetadataContextState>({
    priceIndexes: [],
    priceMarkets: [],
});

export const EnumMetadataProvider = ({ children }: { children: ReactNode }) => {
    const { data } = useQuery<Query>(enumMetadataQuery);

    let priceIndexes: Array<PriceIndex> = [];
    let priceMarkets: Array<PriceMarket> = [];
    if (data) {
        priceIndexes = data.priceIndexes as Array<PriceIndex>;
        priceMarkets = data.priceMarkets as Array<PriceMarket>;
    }

    const value: EnumMetadataContextState = { priceIndexes, priceMarkets };

    return <EnumMetadataContext.Provider value={value}>{children}</EnumMetadataContext.Provider>;
};
