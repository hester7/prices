import { Typography } from "@mui/material";
import { memo } from "react";
import { PricingNode } from "../../gql/codegen/graphql";
import { findClosestAccessibleColor } from "../../utils/a11y/color";
import { getNodeColor, getRtoColor } from "../../utils/color";
import { toPascalCase } from "../../utils/string";

type SymbolProps = {
    pricingNode: PricingNode;
    size?: "tiny" | "small" | "medium" | "large";
    showRto?: boolean;
};

export const Symbol = memo<SymbolProps>(function Symbol({ pricingNode, size = "medium", showRto = false }) {
    const nodeColor = getNodeColor(pricingNode);
    const rtoColor = getRtoColor(pricingNode);

    return (
        <>
            <Typography
                variant="caption"
                sx={(theme) => ({
                    color: findClosestAccessibleColor(nodeColor, theme.palette.background.default, 3),
                    fontSize: theme.typography[`fontSize${toPascalCase(size)}`],
                    fontWeight: theme.typography.fontWeightMedium,
                    lineHeight: 1,
                })}
            >
                {pricingNode.name}
            </Typography>
            {showRto ? (
                <Typography
                    variant="caption"
                    sx={(theme) => ({
                        color: findClosestAccessibleColor(rtoColor, theme.palette.background.default, 3),
                        fontSize: theme.typography[`fontSize${toPascalCase(size)}`],
                        fontWeight: theme.typography.fontWeightMedium,
                        lineHeight: 1,
                    })}
                >
                    {pricingNode.regionalTransmissionOperatorId}
                </Typography>
            ) : null}
        </>
    );
});
